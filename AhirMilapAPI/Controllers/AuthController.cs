using AhirMilap.DAL.CommonService;
using AhirMilap.DAL.IServices;
using AhirMilap.DTO;
using AhirMilap.Entity;
using AhirMilapAPI.Data;
using AhirMilapAPI.Setup; 
using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace AhirMilapAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly UserManager<AppUser> _userManager;
        protected readonly JwtManager _jwtManager;
        protected readonly EmailHelper _emailHelper;
        private readonly ILocalizationService _localizer;
        ILog _logger = LogManager.GetLogger(typeof(AuthController));
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly string _domainPath;

        public AuthController(IAuthService authService, UserManager<AppUser> userManager, JwtManager jwtManager, EmailHelper emailHelper, RoleManager<IdentityRole> roleManager, ILocalizationService localizer, IConfiguration configuration)
        {
            _authService = authService;
            _userManager = userManager;
            _jwtManager = jwtManager;
            _emailHelper = emailHelper;
            _roleManager = roleManager;
            _localizer = localizer;
            _domainPath = configuration["Domain:Path"] ?? string.Empty;
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginReqDTO model)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    user = await _userManager.Users.SingleOrDefaultAsync(u => u.PhoneNumber == model.Email);
                }

                if (user is null)
                    return NotFound(new CommonResponse(Status.NotFound, _localizer.Get("UserNotFound")));

                if (await _userManager.CheckPasswordAsync(user, model.Password))
                {
                    var userRoles = await _userManager.GetRolesAsync(user);

                    var authClaims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Email, user.Email),
                        new Claim(ClaimTypes.Sid, user.Id),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    };

                    foreach (var userRole in userRoles)
                    {
                        authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                    }

                    var getProfile = await _authService.GetMatrimonyProfile(user.Id);
                    if (getProfile.Data != null)
                    {
                        authClaims.Add(new Claim("ProfileId", getProfile.Data.Id.ToString()));
                    }

                    var token = _jwtManager.GetToken(authClaims);

                    UserAccDTO userAccDTO = new UserAccDTO()
                    {
                        id = user.Id,
                        firstName = user.FirstName,
                        email = user.Email,
                        phoneNumber = user.PhoneNumber,
                        UserRolesLst = userRoles,
                        ProfileStep = getProfile.Data != null ? getProfile.Data.ProfileStep : 0,
                    };

                    LoginResDTO loginUser = new LoginResDTO()
                    {
                        Tokens = token,
                        Users = userAccDTO,
                    };

                    return Ok(new CommonResponse(Status.Success, _localizer.Get("Success"), loginUser));

                }

                return Unauthorized(new CommonResponse(Status.Error, _localizer.Get("InvalidEmailOrPassword")));
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                return StatusCode(500, new CommonResponse(Status.Error, ex.Message));
            }
        }


        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterReqDTO model)
        {
            try
            {
                AppUser userByEmail = null;
                if (model.Email != null)
                {
                    userByEmail = await _userManager.FindByEmailAsync(model.Email);
                }
                else
                {
                    userByEmail = await _userManager.Users.Where(x => x.PhoneNumber == model.Phone).FirstOrDefaultAsync();
                }

                int _min = 100000;
                int _max = 999999;
                Random _rdm = new Random();
                var OTP = _rdm.Next(_min, _max);

                if (userByEmail != null && userByEmail.EmailConfirmed)
                {
                    return Conflict(new CommonResponse(Status.Error, _localizer.Get("UserAlreadyExists")));
                }
                else if (model.IsResend && userByEmail is not null)
                {
                    userByEmail.OTP = OTP;
                    userByEmail.OTPExpiry = DateTime.Now.AddMinutes(3);
                    userByEmail.UpdatedOn = DateTime.Now;
                    var updateUser = await _userManager.UpdateAsync(userByEmail);
                    if (updateUser != null && !updateUser.Succeeded)
                    {
                        return Conflict(new CommonResponse(Status.Error, string.Join(", ", updateUser.Errors.Select(error => error.Description).ToArray())));
                    }
                }
                else
                {
                    if (userByEmail == null)
                    {
                        userByEmail = new()
                        {
                            Email = model.Email,
                            UserName = model.Email != null ? model.Email : model.Phone,
                            PhoneNumber = model?.Phone,
                            OTP = OTP,
                            OTPExpiry = DateTime.Now.AddMinutes(3),
                            SecurityStamp = Guid.NewGuid().ToString(),
                            CreatedOn = DateTime.Now,
                        };

                        var result = await _userManager.CreateAsync(userByEmail, model.Password);
                        if (result != null && !result.Succeeded)
                        {
                            return Conflict(new CommonResponse(Status.Error, string.Join(", ", result.Errors.Select(error => error.Description).ToArray())));
                        }
                    }

                    userByEmail.OTP = OTP;
                    userByEmail.OTPExpiry = DateTime.Now.AddMinutes(3);
                    userByEmail.UpdatedOn = DateTime.Now;

                    await _userManager.UpdateAsync(userByEmail);
                }

                bool sent = false;
                if (model.Email != null)
                {
                    var file = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplates", "SentOTP.html");
                    string content = System.IO.File.ReadAllText(file);
                    content = content.Replace("{name}", "User").Replace("{OTP}", OTP.ToString());
                    sent = _emailHelper.SendEmail(new MaileModelHelper(model.Email, "", _localizer.Get("VerifyOTP"), content, true, null, null));
                }
                else if (model.Phone != null)
                {
                    sent = _emailHelper.SendSMS(model.Phone, $"{OTP} is your Ahir Milap verification code. This is valid for the next 3 minutes.");
                }

                if (sent)
                {
                    return Ok(new CommonResponse(Status.Success, _localizer.Get("OTPSentSuccessfully")));
                }

                return Ok(new CommonResponse(Status.Error, _localizer.Get("OTPFailed")));
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                return StatusCode(500, new CommonResponse(Status.Error, ex.Message));
            }
        }


        [HttpPost("addRoles")] // for the add the role name Admin & User
        public async Task<IActionResult> AddRoles()
        {
            try
            {
                var x = await _roleManager.RoleExistsAsync("Admin");
                if (!x)
                {
                    var role1 = new IdentityRole();
                    role1.Name = "Admin";
                    await _roleManager.CreateAsync(role1);
                }
                var x1 = await _roleManager.RoleExistsAsync("User");
                if (!x1)
                {
                    var role2 = new IdentityRole();
                    role2.Name = "User";
                    await _roleManager.CreateAsync(role2);
                }

                return Ok(new CommonResponse(Status.Success, _localizer.Get("RolesCreatedSuccessfully")));
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                throw;
            }
        }


        [HttpPost("forgotPassword")]
        public async Task<IActionResult> ForgotPassword(ForgotRequestDTO model)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null) return NotFound(new CommonResponse(Status.NotFound, _localizer.Get("UserNotFound")));

                var OTP = Constants.GenerateOTP(6);
                var file = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplates", "ForgotPassword.html");
                string content = System.IO.File.ReadAllText(file);
                content = content.Replace("{name}", "User").Replace("{OTP}", OTP.ToString());
                var sent = _emailHelper.SendEmail(new MaileModelHelper(model.Email, "", "Forgot Password", content, true, null, null));
                if (sent)
                {
                    user.OTP = Convert.ToInt32(OTP);
                    user.OTPExpiry = DateTime.Now.AddMinutes(3);
                    var updateUser = await _userManager.UpdateAsync(user);
                    if (!updateUser.Succeeded)
                    {
                        return Ok(new CommonResponse(Status.Error, string.Join(", ", updateUser.Errors.Select(error => error.Description).ToArray())));
                    }
                }

                return Ok(new CommonResponse(Status.Success, _localizer.Get("OTPSentSuccessfullyCheckEmail")));
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                return StatusCode(500, new CommonResponse(Status.Error, ex.Message));
            }
        }


        [HttpPost("updatePassword")]
        public async Task<IActionResult> UpdatePassword(UpdatePasswordRequestDTO model)
        {
            try
            {
                AppUser userByEmail = null;
                if (model.Email != null)
                {
                    userByEmail = await _userManager.FindByEmailAsync(model.Email);
                }

                if (userByEmail is null)
                {
                    return Ok(new CommonResponse(Status.Error, _localizer.Get("UserNotExistsPleaseSignup")));
                }

                userByEmail.PasswordHash = _userManager.PasswordHasher.HashPassword(userByEmail, model.Password);
                userByEmail.UpdatedOn = DateTime.Now;
                userByEmail.SecurityStamp = Guid.NewGuid().ToString();
                var updateUser = await _userManager.UpdateAsync(userByEmail);
                if (updateUser != null && !updateUser.Succeeded)
                {
                    return Ok(new CommonResponse(Status.Error, string.Join(", ", updateUser.Errors.Select(error => error.Description).ToArray())));
                }
                return Ok(new CommonResponse(Status.Success, _localizer.Get("PasswordUpdatedSuccessfully")));
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                throw;
            }
        }


        [HttpPost("sentOTPByEmailOrMobile")]
        public async Task<IActionResult> SentOTPByEmailOrMobile(SendOtpRequest model)
        {
            try
            {
                AppUser userByEmail = null;
                if (model.Email != null)
                {
                    userByEmail = await _userManager.FindByEmailAsync(model.Email);
                }
                else
                {
                    userByEmail = await _userManager.Users.Where(x => x.PhoneNumber == model.Phone).FirstOrDefaultAsync();
                }

                int _min = 100000;
                int _max = 999999;
                Random _rdm = new Random();
                var OTP = _rdm.Next(_min, _max);

                if (userByEmail != null && userByEmail.EmailConfirmed)
                {
                    return Conflict(new CommonResponse(Status.Error, _localizer.Get("UserAlreadyExists")));
                }
                else if (model.IsResend && userByEmail is not null)
                {
                    userByEmail.OTP = OTP;
                    userByEmail.OTPExpiry = DateTime.Now.AddMinutes(3);
                    userByEmail.UpdatedOn = DateTime.Now;
                    var updateUser = await _userManager.UpdateAsync(userByEmail);
                    if (updateUser != null && !updateUser.Succeeded)
                    {
                        return Conflict(new CommonResponse(Status.Error, string.Join(", ", updateUser.Errors.Select(error => error.Description).ToArray())));
                    }
                }
                else
                {
                    if (userByEmail == null)
                    {
                        userByEmail = new()
                        {
                            Email = model.Email,
                            UserName = model.Email != null ? model.Email : model.Phone,
                            PhoneNumber = model.Phone,
                            OTP = OTP,
                            OTPExpiry = DateTime.Now.AddMinutes(3),
                            SecurityStamp = Guid.NewGuid().ToString(),
                            CreatedOn = DateTime.Now,
                        };

                        var result = await _userManager.CreateAsync(userByEmail);
                        if (result != null && !result.Succeeded)
                        {
                            return Conflict(new CommonResponse(Status.Error, string.Join(", ", result.Errors.Select(error => error.Description).ToArray())));
                        }
                    }

                    userByEmail.OTP = OTP;
                    userByEmail.OTPExpiry = DateTime.Now.AddMinutes(3);
                    userByEmail.UpdatedOn = DateTime.Now;

                    await _userManager.UpdateAsync(userByEmail);
                }

                bool sent = false;
                if (model.Email != null)
                {
                    var file = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplates", "SentOTP.html");
                    string content = System.IO.File.ReadAllText(file);
                    content = content.Replace("{name}", "User").Replace("{OTP}", OTP.ToString());
                    sent = _emailHelper.SendEmail(new MaileModelHelper(model.Email, "", _localizer.Get("VerifyOTP"), content, true, null, null));
                }
                else if (model.Phone != null)
                {
                    sent = _emailHelper.SendSMS(model.Phone, $"{OTP} is your Ahir Milap verification code. This is valid for the next 3 minutes.");
                }

                if (sent)
                {
                    return Ok(new CommonResponse(Status.Success, _localizer.Get("OTPSentSuccessfully")));
                }

                return Ok(new CommonResponse(Status.Error, _localizer.Get("OTPFailed")));
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                return StatusCode(500, new CommonResponse(Status.Error, ex.Message));
            }
        }


        [HttpPost("verifyOTPByEmailOrMobile")]
        public async Task<IActionResult> VerifyOTPByEmailOrMobile(VerifyOtpRequest model)
        {
            try
            {
                AppUser userByEmail = null;
                if (model.Email != null)
                {
                    userByEmail = await _userManager.FindByEmailAsync(model.Email);
                }
                else
                {
                    userByEmail = await _userManager.Users.Where(x => x.PhoneNumber == model.Email).FirstOrDefaultAsync();
                }

                if (userByEmail is null)
                {
                    return NotFound(new CommonResponse(Status.Error, _localizer.Get("UserNotExistsPleaseSignup")));
                }

                var res = await _authService.VerifyOTP(model.Email, model.otp);
                if (res.Data == 1)
                {
                    userByEmail.EmailConfirmed = true;
                    userByEmail.UpdatedOn = DateTime.Now;
                    var updateUser = await _userManager.UpdateAsync(userByEmail);
                    if (updateUser != null && !updateUser.Succeeded)
                    {
                        return Conflict(new CommonResponse(Status.Error, string.Join(", ", updateUser.Errors.Select(error => error.Description).ToArray())));
                    }

                    if (!await _userManager.IsInRoleAsync(userByEmail, "User"))
                    {
                        await _userManager.AddToRoleAsync(userByEmail, "User"); // while set admin role then change to "Admin" default "User"
                    }

                    var userRoles = await _userManager.GetRolesAsync(userByEmail);
                    var authClaims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Email, userByEmail.Email),
                        new Claim(ClaimTypes.Sid, userByEmail.Id),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    };

                    foreach (var userRole in userRoles)
                    {
                        authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                    }

                    var getProfile = await _authService.GetMatrimonyProfile(userByEmail.Id);
                    if (getProfile.Data != null)
                    {
                        authClaims.Add(new Claim("ProfileId", getProfile.Data.Id.ToString()));
                    }

                    var token = _jwtManager.GetToken(authClaims);

                    UserAccDTO userAccDTO = new UserAccDTO()
                    {
                        id = userByEmail.Id,
                        firstName = userByEmail.FirstName,
                        email = userByEmail.Email,
                        phoneNumber = userByEmail.PhoneNumber,
                        UserRolesLst = userRoles,
                        ProfileStep = getProfile.Data != null ? getProfile.Data.ProfileStep : 0,
                    };

                    LoginResDTO loginUser = new LoginResDTO()
                    {
                        Tokens = token,
                        Users = userAccDTO,
                    };

                    return Ok(new CommonResponse(Status.Success, _localizer.Get("OTPVerifiedSuccessfully"), loginUser));
                }
                else if (res.Data == 2)
                    return Conflict(new CommonResponse(Status.Error, _localizer.Get("OTPExpired")));
                else
                    return Conflict(new CommonResponse(Status.Error, _localizer.Get("InvalidOTP")));

            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                return StatusCode(500, new CommonResponse(Status.Error, ex.Message));
            }
        }

        [Authorize]
        [HttpPost("changePassword")]
        public async Task<IActionResult> ChangePassword(ChangePassReqDTO model)
        {
            try
            {
                var userId = User.GetUserId();
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return NotFound(new CommonResponse(Status.Error, _localizer.Get("UserNotExistsPleaseSignup")));
                }

                IdentityResult result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
                if (result != null && !result.Succeeded)
                {
                    return Conflict(new CommonResponse(Status.Error, _localizer.Get("OldPasswordInvalid")));
                }

                return Ok(new CommonResponse(Status.Success, _localizer.Get("PasswordUpdatedSuccessfully")));
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                throw;
            }
        }


        // Create Profile
        [Authorize]
        [HttpPost("createMatrimonyProfile")]
        public async Task<IActionResult> CreateMatrimonyProfile(CreateProfileDTO dto)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(User.GetUserId());
                if (user is null)
                {
                    return NotFound(new CommonResponse(Status.Error, _localizer.Get("UserNotExistsPleaseSignup")));
                }

                MatrimonyProfile model = new MatrimonyProfile()
                {
                    UserId = User.GetUserId(),
                    ProfileFor = dto.ProfileFor,
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    Gender = dto.Gender,
                    DOB = dto.DOB,
                    Age = dto.Age, // DateTime.Now.Year - dto.DOB.Year,
                    MaritalStatus = dto.MaritalStatus,
                    State = dto.State,
                    District = dto.District,
                    AboutMe = dto.AboutMe,
                    PageFlag = "Basic"
                };

                var res = await _authService.AddMatrimonyProfile(model);
                if (res.Data != null)
                {
                    return Ok(new CommonResponse(Status.Success, _localizer.Get("DataSavedSuccessfully"), res.Data));
                }
                return Conflict(new CommonResponse(Status.Error, _localizer.Get("Error")));
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                return StatusCode(500, new CommonResponse(Status.Error, ex.Message));
            }
        }

        [Authorize]
        [HttpPut("religion")]
        public async Task<IActionResult> UpdateReligion([FromForm] ReligionDTO dto)
        {
            try
            {
                string userId = User.GetUserId();
                MatrimonyProfile model = new MatrimonyProfile()
                {
                    UserId = User.GetUserId(),
                    Religion = dto.Religion,
                    Caste = dto.Caste,
                    SubCaste = dto.SubCaste,
                    LanguageKnow = dto.LanguageKnow,
                    PageFlag = "Religion"
                };

                var getProfile = await _authService.GetMatrimonyProfile(userId);
                if (dto.AadharFile1 != null)
                {
                    var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "Attachment", userId, "Profile");
                    var saveDir = Path.Combine("Attachment", userId, "Profile");

                    if (getProfile.Data != null && getProfile.Data.AadhaarFilePath1 != null)
                    {
                        getProfile.Data.AadhaarFilePath1 = getProfile.Data.AadhaarFilePath1.Replace(_domainPath, "");
                        var removePath = Path.Combine(Directory.GetCurrentDirectory(), getProfile.Data.AadhaarFilePath1);
                        if (System.IO.File.Exists(removePath)) System.IO.File.Delete(removePath);
                    }

                    if (!Directory.Exists(uploadPath))
                    {
                        Directory.CreateDirectory(uploadPath);
                    }

                    var fileExt = Path.GetExtension(dto.AadharFile1.FileName);
                    FileSupportedType fileType = new();
                    if (!fileType.IsAllowPhotoFile(fileExt)) return Ok(new CommonResponse(Status.Error, $"{fileExt} File which you try to upload is not supported."));

                    FileSupportedSize fileSize = new();
                    var isAllowSize = await fileSize.IsAllowPhotoSize(dto.AadharFile1.Length);
                    if (!isAllowSize.Item1) return Ok(new CommonResponse(Status.Error, $"File you try to uploaded is larger than supported size, Max File size is {isAllowSize.Item2} MB."));


                    var fileName = Path.GetFileNameWithoutExtension(dto.AadharFile1.FileName) + "_" + DateTime.Now.ToString("ddMMyyyyHHmmss") + fileExt;
                    string path = Path.Combine(uploadPath, fileName);
                    string savePath = Path.Combine(saveDir, fileName);
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await dto.AadharFile1.CopyToAsync(stream);
                        await stream.FlushAsync();
                    }
                    model.AadhaarFilePath1 = savePath;
                }

                if (dto.AadharFile2 != null)
                {
                    var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "Attachment", userId, "Profile");
                    var saveDir = Path.Combine("Attachment", userId, "Profile");

                    if (getProfile.Data != null && getProfile.Data.AadhaarFilePath2 != null)
                    {
                        getProfile.Data.AadhaarFilePath2 = getProfile.Data.AadhaarFilePath2.Replace(_domainPath, "");
                        var removePath = Path.Combine(Directory.GetCurrentDirectory(), getProfile.Data.AadhaarFilePath2);
                        if (System.IO.File.Exists(removePath)) System.IO.File.Delete(removePath);
                    }

                    if (!Directory.Exists(uploadPath))
                    {
                        Directory.CreateDirectory(uploadPath);
                    }

                    var fileExt = Path.GetExtension(dto.AadharFile2.FileName);
                    FileSupportedType fileType = new();
                    if (!fileType.IsAllowPhotoFile(fileExt)) return Ok(new CommonResponse(Status.Error, $"{fileExt} File which you try to upload is not supported."));

                    FileSupportedSize fileSize = new();
                    var isAllowSize = await fileSize.IsAllowPhotoSize(dto.AadharFile2.Length);
                    if (!isAllowSize.Item1) return Ok(new CommonResponse(Status.Error, $"File you try to uploaded is larger than supported size, Max File size is {isAllowSize.Item2} MB."));


                    var fileName = Path.GetFileNameWithoutExtension(dto.AadharFile2.FileName) + "_" + DateTime.Now.ToString("ddMMyyyyHHmmss") + fileExt;
                    string path = Path.Combine(uploadPath, fileName);
                    string savePath = Path.Combine(saveDir, fileName);
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await dto.AadharFile2.CopyToAsync(stream);
                        await stream.FlushAsync();
                    }
                    model.AadhaarFilePath2 = savePath;
                }

                var res = await _authService.AddMatrimonyProfile(model);
                if (res.Data != null)
                {
                    return Ok(new CommonResponse(Status.Success, _localizer.Get("DataSavedSuccessfully"), res.Data));
                }
                return Conflict(new CommonResponse(Status.Error, _localizer.Get("Error")));
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                return StatusCode(500, new CommonResponse(Status.Error, ex.Message));
            }
        }

        [Authorize]
        [HttpPut("uploadProfiles")]
        public async Task<IActionResult> UploadProfile(List<IFormFile> model)
        {
            try
            {
                string userId = User.GetUserId();

                if (model == null || model.Count == 0)
                    return Conflict(new CommonResponse(Status.Error, _localizer.Get("PleaseSelectFiles")));

                var getProfile = await _authService.GetMatrimonyProfile(userId);
                if (getProfile.Data != null && getProfile.Data.ProfilePhotos != null && getProfile.Data.ProfilePhotos.Count > 0)
                {
                    if (getProfile.Data.ProfileFilePath != null)
                    {
                        getProfile.Data.ProfileFilePath = getProfile.Data.ProfileFilePath.Replace(_domainPath, "");
                        var removePath = Path.Combine(Directory.GetCurrentDirectory(), getProfile.Data.ProfileFilePath);
                        if (System.IO.File.Exists(removePath)) System.IO.File.Delete(removePath);
                    }

                    foreach (var photo in getProfile.Data.ProfilePhotos)
                    {
                        photo.PhotoPath = photo.PhotoPath.Replace(_domainPath, "");
                        var removePath = Path.Combine(Directory.GetCurrentDirectory(), photo.PhotoPath);
                        if (System.IO.File.Exists(removePath)) System.IO.File.Delete(removePath);
                    }
                }

                var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "Attachment", userId, "Profile");
                var saveDir = Path.Combine("Attachment", userId, "Profile");

                if (!Directory.Exists(uploadPath))
                    Directory.CreateDirectory(uploadPath);

                List<string> savedFiles = new List<string>();

                FileSupportedType fileType = new FileSupportedType();
                FileSupportedSize fileSize = new FileSupportedSize();

                foreach (var file in model)
                {
                    if (file == null || file.Length == 0)
                        continue;

                    var ext = Path.GetExtension(file.FileName).ToLower();

                    if (!fileType.IsAllowPhotoFile(ext))
                        continue;

                    var allowSize = await fileSize.IsAllowPhotoSize(file.Length);

                    if (!allowSize.Item1)
                        continue;

                    var fileName = Path.GetFileNameWithoutExtension(file.FileName) + "_" + Guid.NewGuid().ToString("N") + ext;

                    string fullPath = Path.Combine(uploadPath, fileName);
                    string dbPath = Path.Combine(saveDir, fileName);

                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                        await stream.FlushAsync();
                    }

                    savedFiles.Add(dbPath);
                }

                if (savedFiles.Count == 0)
                    return Conflict(new CommonResponse(Status.Error, _localizer.Get("NoValidFilesUploaded")));

                MatrimonyProfile model1 = new MatrimonyProfile()
                {
                    UserId = User.GetUserId(),
                    ProfileFilePath = string.Join(",", savedFiles),
                    PageFlag = "Profile"
                };

                var res = await _authService.AddMatrimonyProfile(model1);
                if (res.Data != null)
                {
                    return Ok(new CommonResponse(Status.Success, _localizer.Get("DataSavedSuccessfully"), res.Data));
                }
                return Conflict(new CommonResponse(Status.Error, _localizer.Get("Error")));
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                return StatusCode(500, new CommonResponse(Status.Error, ex.Message));
            }
        }

        [Authorize]
        [HttpPut("education")]
        public async Task<IActionResult> Education(EducationDTO dto)
        {
            try
            {
                MatrimonyProfile model = new MatrimonyProfile()
                {
                    UserId = User.GetUserId(),
                    HighestEducation = dto.HighestEducation,
                    CollegeUniversity = dto.CollegeUniversity,
                    Occupation = dto.Occupation,
                    CompanyName = dto.CompanyName,
                    AnnualIncome = dto.AnnualIncome,
                    PageFlag = "Education"
                };

                var res = await _authService.AddMatrimonyProfile(model);
                if (res.Data != null)
                {
                    return Ok(new CommonResponse(Status.Success, _localizer.Get("DataSavedSuccessfully"), res.Data));
                }
                return Conflict(new CommonResponse(Status.Error, _localizer.Get("Error")));
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                return StatusCode(500, new CommonResponse(Status.Error, ex.Message));
            }
        }

        [Authorize]
        [HttpPut("lifestyle")]
        public async Task<IActionResult> Lifestyle(LifestyleDTO dto)
        {
            try
            {
                MatrimonyProfile model = new MatrimonyProfile()
                {
                    UserId = User.GetUserId(),
                    HeightCm = dto.HeightCm,
                    WeightKg = dto.WeightKg,
                    Complexion = dto.Complexion,
                    PhysicalDisability = dto.PhysicalDisability,
                    DisabilityDetails = dto.DisabilityDetails,
                    PageFlag = "Lifestyle"
                };

                var res = await _authService.AddMatrimonyProfile(model);
                if (res.Data != null)
                {
                    return Ok(new CommonResponse(Status.Success, _localizer.Get("DataSavedSuccessfully"), res.Data));
                }
                return Conflict(new CommonResponse(Status.Error, _localizer.Get("Error")));
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                return StatusCode(500, new CommonResponse(Status.Error, ex.Message));
            }
        }

        [Authorize]
        [HttpPut("familyDetails")]
        public async Task<IActionResult> FamilyDetails(FamilyDTO dto)
        {
            try
            {
                MatrimonyProfile model = new MatrimonyProfile()
                {
                    UserId = User.GetUserId(),
                    FatherName = dto.FatherName,
                    FatherOccupation = dto.FatherOccupation,
                    MotherName = dto.MotherName,
                    MotherOccupation = dto.MotherOccupation,
                    FamilyType = dto.FamilyType,
                    PageFlag = "Family",
                    Siblings = dto.Siblings
                };

                var res = await _authService.AddMatrimonyProfile(model);
                if (res.Data != null)
                {
                    return Ok(new CommonResponse(Status.Success, _localizer.Get("DataSavedSuccessfully"), res.Data));
                }
                return Conflict(new CommonResponse(Status.Error, _localizer.Get("Error")));
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                return StatusCode(500, new CommonResponse(Status.Error, ex.Message));
            }
        }

        [Authorize]
        [HttpGet("getMatrimonyProfile")]
        public async Task<IActionResult> GetMatrimonyProfile()
        {
            var res = await _authService.GetMatrimonyProfile(User.GetUserId());
            if (res.Data != null)
            {
                return Ok(new CommonResponse(Status.Success, _localizer.Get("DataGatedSuccessfully"), res.Data));
            }
            return Conflict(new CommonResponse(Status.Error, _localizer.Get("Error")));
        }

        [Authorize]
        [HttpGet("getStates")]
        public async Task<IActionResult> GetStates()
        {
            try
            {
                var res = await _authService.GetStates();
                if (res.Res != null)
                {
                    return Ok(new CommonResponse(Status.Success, _localizer.Get("Success"), res.Res));
                }

                return Conflict(new CommonResponse(Status.NotFound, _localizer.Get("NotFound")));
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                return StatusCode(500, new CommonResponse(Status.Error, ex.Message));
            }
        }

        [Authorize]
        [HttpGet("getDistrictByStateId/{stateId}")]
        public async Task<IActionResult> GetDistrictByStateId(int stateId)
        {
            try
            {
                var res = await _authService.GetDistrictByStateId(stateId);
                if (res.Res != null)
                {
                    return Ok(new CommonResponse(Status.Success, _localizer.Get("Success"), res.Res));
                }

                return Conflict(new CommonResponse(Status.NotFound, _localizer.Get("NotFound")));
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                return StatusCode(500, new CommonResponse(Status.Error, ex.Message));
            }
        }
        [Authorize]
        [HttpGet("getProfession")]
        public async Task<IActionResult> GetProfession()
        {
            try
            {
                var res = await _authService.GetProfession();
                if (res.Res != null)
                {
                    return Ok(new CommonResponse(Status.Success, _localizer.Get("Success"), res.Res));
                }

                return Conflict(new CommonResponse(Status.NotFound, _localizer.Get("Error")));
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                return StatusCode(500, new CommonResponse(Status.Error, ex.Message));
            }
        }

        [Authorize]
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            return Ok(new CommonResponse(Status.Success, _localizer.Get("LoggedOutSuccessfully")));
        }

        [Authorize]
        [HttpPost("deleteProfilePhoto/{id}")]
        public async Task<IActionResult> DeleteProfilePhoto(Guid? id)
        {
            try
            {
                string userId = User.GetUserId();
                var getProfile = await _authService.GetMatrimonyProfile(userId);
                if (getProfile.Data != null && getProfile.Data.ProfilePhotos.Count > 0)
                {
                    var photo = getProfile.Data.ProfilePhotos.Where(x => x.Id == id).FirstOrDefault();
                    if (photo != null)
                    {
                        photo.PhotoPath = photo.PhotoPath.Replace(_domainPath, "");
                        var removePath = Path.Combine(Directory.GetCurrentDirectory(), photo.PhotoPath);
                        if (System.IO.File.Exists(removePath)) System.IO.File.Delete(removePath);
                    }
                }

                var res = await _authService.DeleteProfilePhoto(id);
                if (res.Data)
                {
                    return Ok(new CommonResponse(Status.Success, _localizer.Get("DataDeletedSuccessfully")));
                }
                return Conflict(new CommonResponse(Status.NotFound, _localizer.Get("NotFound")));
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                return StatusCode(500, new CommonResponse(Status.Error, ex.Message));
            }
        }

        [Authorize]
        [HttpPut("uploadProfilePhoto")]
        public async Task<IActionResult> UploadProfilePhoto(IFormFile file, int sortOrder)
        {
            try
            {
                FileSupportedType fileType = new FileSupportedType();
                FileSupportedSize fileSize = new FileSupportedSize();

                var ext = Path.GetExtension(file.FileName).ToLower();

                if (!fileType.IsAllowPhotoFile(ext))
                    return Conflict(new CommonResponse(Status.Error, _localizer.Get("FileNotSupported")));

                var allowSize = await fileSize.IsAllowPhotoSize(file.Length);
                if (!allowSize.Item1)
                    return Conflict(new CommonResponse(Status.Error, _localizer.Get("FileSizeExceeded")));

                string userId = User.GetUserId();
                var getProfile = await _authService.GetMatrimonyProfile(userId);
                if (getProfile.Data != null && getProfile.Data.ProfilePhotos != null)
                {
                    if (getProfile.Data.ProfileFilePath != null && sortOrder == 0)
                    {
                        getProfile.Data.ProfileFilePath = getProfile.Data.ProfileFilePath.Replace(_domainPath, ""); 
                        var removePath = Path.Combine(Directory.GetCurrentDirectory(), getProfile.Data.ProfileFilePath);
                        if (System.IO.File.Exists(removePath)) System.IO.File.Delete(removePath);
                    }
                    else if (getProfile.Data.ProfilePhotos.Count > 0 && sortOrder > 0)
                    {
                        var photo = getProfile.Data.ProfilePhotos.Where(x => x.SortOrder == sortOrder).FirstOrDefault();
                        if (photo != null)
                        {
                            photo.PhotoPath = photo.PhotoPath.Replace(_domainPath, "");
                            var removePath = Path.Combine(Directory.GetCurrentDirectory(), photo.PhotoPath);
                            if (System.IO.File.Exists(removePath)) System.IO.File.Delete(removePath);
                        }
                    }
                }

                var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "Attachment", userId, "Profile");
                var saveDir = Path.Combine("Attachment", userId, "Profile");

                if (!Directory.Exists(uploadPath))
                    Directory.CreateDirectory(uploadPath);

                var fileName = Path.GetFileNameWithoutExtension(file.FileName) + "_" + Guid.NewGuid().ToString("N") + ext;
                string fullPath = Path.Combine(uploadPath, fileName);
                string dbPath = Path.Combine(saveDir, fileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                    await stream.FlushAsync();
                }

                MatrimonyProfilePhotos profilePhotos = new MatrimonyProfilePhotos()
                {
                    PhotoPath = dbPath,
                    SortOrder = sortOrder,
                    CreatedBy = userId,
                };

                var res = await _authService.UpdateProfilePhoto(profilePhotos);
                if (res.Data != null)
                {
                    return Ok(new CommonResponse(Status.Success, _localizer.Get("DataSavedSuccessfully"), res.Data));
                }
                return Conflict(new CommonResponse(Status.Error, _localizer.Get("Error")));
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                return StatusCode(500, new CommonResponse(Status.Error, ex.Message));
            }
        }
    }
}