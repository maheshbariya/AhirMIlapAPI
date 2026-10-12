using AhirMilap.DAL.IServices;
using AhirMilap.DTO;
using AhirMilap.Entity;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using Google.Cloud.Firestore.V1;
using Grpc.Auth;
using Grpc.Core;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace AhirMilap.DAL.CommonService
{
    public class FirebaseNotificationService
    {
        private readonly IFirebaseService _firebaseSerice;
        FirestoreDb db;
        private readonly ILocalizationService _localizer;

        public FirebaseNotificationService(IConfiguration configuration, IFirebaseService firebaseSerice, ILocalizationService localizer)
        {
            _firebaseSerice = firebaseSerice;

            GoogleCredential credential;
            string configPath = Path.Combine(Directory.GetCurrentDirectory(), "CommonService", "ahirmilap-firebase-adminsdk.json");
            using (var stream = new FileStream(configPath, FileMode.Open, FileAccess.Read))
            {
                credential = GoogleCredential.FromStream(stream);
                //.CreateScoped("https://www.googleapis.com/auth/firebase.messaging");
            }

            if (FirebaseApp.DefaultInstance == null)
            {
                FirebaseApp.Create(new AppOptions
                {
                    Credential = credential,
                    ProjectId = "ahirmilap-83996"
                });
            }

            ChannelCredentials channelCredentials = credential.ToChannelCredentials();
            FirestoreClientBuilder clientBuilder = new FirestoreClientBuilder
            {
                ChannelCredentials = channelCredentials
            };
            db = FirestoreDb.Create("ahirmilap-83996", clientBuilder.Build());
            _localizer = localizer;
        }

        public async Task<FirebaseMsgApiRes> SendNotificationAsync(Notifications model, object stroryId = null)
        {
            try
            {
                var getToken = await _firebaseSerice.GetFirebaseToken(model.ReceiverProfileId, model.SenderProfileId);
                if (getToken.Res == null)
                {
                    return new FirebaseMsgApiRes
                    {
                        success = false,
                        name = "Message not sent, due to not found token."
                    };
                }

                //Save notification  
                if (model.NotificationType == "PROFILE_LIKE")
                {
                    model.Title = getToken.Res[0].ReceiverName;
                    model.Message = string.Format(_localizer.Get("ProfileLikeMsg"), getToken.Res[0].SenderName);
                }
                else if (model.NotificationType == "PROFILE_VIEW")
                {
                    model.Title = getToken.Res[0].ReceiverName;
                    model.Message = string.Format(_localizer.Get("ProfileViewMsg"), getToken.Res[0].SenderName);
                }
                else if (model.NotificationType == "REQUEST_SENT")
                {
                    model.Title = getToken.Res[0].ReceiverName;
                    model.Message = string.Format(_localizer.Get("ReqSentMsg"), getToken.Res[0].SenderName);
                }
                else if (model.NotificationType == "REQUEST_ACCEPTED")
                {
                    model.Title = getToken.Res[0].ReceiverName;
                    model.Message = string.Format(_localizer.Get("ReqAcceptedMsg"), getToken.Res[0].SenderName); 
                }
                else if (model.NotificationType == "NEW_MESSAGE")
                {
                    model.Title = getToken.Res[0].SenderName;
                }
                else if (model.NotificationType == "PROFILE_VERIFICATION")
                {
                    model.Title = getToken.Res[0].ReceiverName;
                    model.Message = string.Format(_localizer.Get("ProfileVerifiedMsg"), getToken.Res[0].SenderName); 
                }
                else if (model.NotificationType == "PROFILE_UPDATES")
                {
                    model.Title = getToken.Res[0].ReceiverName;
                    model.Message = string.Format(_localizer.Get("ProfileUpdateMsg"), getToken.Res[0].SenderName);
                }

                if (model.NotificationType != "NEW_MESSAGE")
                    await _firebaseSerice.SaveNotifications(model);

                stroryId = new
                {
                    senderId = model.SenderProfileId,
                    receiverId = model.ReceiverProfileId,
                    type = model.NotificationType
                };

                List<string> tokens = getToken.Res.Where(x => !string.IsNullOrEmpty(x.FcmToken)).Select(x => x.FcmToken!).Distinct().ToList();

                var message = new Message
                {
                    Notification = new Notification
                    {
                        Title = model.Title,
                        Body = model.Message,
                    },
                    Data = new Dictionary<string, string>
                    {
                        { "story_id", JsonConvert.SerializeObject(stroryId) }
                    }
                };
                var multicastMessage = new MulticastMessage
                {
                    Tokens = tokens,
                    Notification = message.Notification,
                    Data = message.Data
                };
                var response = await FirebaseMessaging.DefaultInstance.SendEachForMulticastAsync(multicastMessage);
                if (response.FailureCount > 0)
                {
                    var tokenCsv = string.Join(",",
                        response.Responses
                            .Select((res, index) => new { res, index })
                            .Where(x => !x.res.IsSuccess)
                            .Select(x => tokens[x.index])
                    );

                    if (!string.IsNullOrEmpty(tokenCsv))
                    {
                        await _firebaseSerice.DeleteFirebaseTokenByToken(tokenCsv);
                    }
                }

                return new FirebaseMsgApiRes
                {
                    success = true,
                    name = $"{response.SuccessCount} message sent successfully."
                };
            }
            catch (Exception ex)
            {
                var resobj = new FirebaseMsgApiRes()
                {
                    success = false,
                    name = ex.Message.ToString(),
                };
                return resobj;
            }
        }

    }
}
