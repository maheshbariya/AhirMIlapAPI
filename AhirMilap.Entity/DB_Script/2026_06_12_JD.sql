-- SP 1: Get active plans (used by GET /api/subscription/plans)
CREATE OR ALTER PROCEDURE usp_GetActivePlans
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, Name, GoogleSku, Price, Currency,
           DurationDays, Features, IsActive
    FROM   SubscriptionPlans
    WHERE  IsActive = 1
    ORDER  BY Price ASC;
END;
GO

-- SP 2: Get user's active subscription (GET /api/subscription/status)
CREATE OR ALTER PROCEDURE usp_GetActiveSubscriptionByUser
    @ProfileId UniqueIdentifier
AS
BEGIN
    SET NOCOUNT ON;
    SELECT TOP 1
        us.Id, us.ProfileId, us.PlanId,
        us.GoogleOrderId, us.PurchaseToken,
        us.Status, us.StartDate, us.ExpiryDate, us.AutoRenew,
        sp.Name AS PlanName
    FROM   UserSubscriptions us
    JOIN   SubscriptionPlans sp ON sp.Id = us.PlanId
    WHERE  us.ProfileId     = @ProfileId
      AND  us.Status     = 'Active'
      AND  us.ExpiryDate > GETUTCDATE()
    ORDER  BY us.ExpiryDate DESC;
END;
GO

-- SP 3: Get subscription by purchase token (used by webhook)
CREATE OR ALTER PROCEDURE usp_GetSubscriptionByToken
    @PurchaseToken NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT TOP 1 * FROM UserSubscriptions
    WHERE PurchaseToken = @PurchaseToken;
END;
GO

-- SP 4: Upsert subscription (POST /api/subscription/verify)
-- Updates existing Active row if found, else inserts new row
CREATE OR ALTER PROCEDURE usp_UpsertUserSubscription
    @ProfileId     UniqueIdentifier,
    @PlanId        INT,
    @GoogleOrderId NVARCHAR(200),
    @PurchaseToken NVARCHAR(MAX),
    @Status        NVARCHAR(20),
    @StartDate     DATETIME2,
    @ExpiryDate    DATETIME2,
    @AutoRenew     BIT
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (
        SELECT 1 FROM UserSubscriptions
        WHERE ProfileId = @ProfileId AND Status = 'Active'
    )
    BEGIN
        UPDATE UserSubscriptions SET
            PlanId        = @PlanId,
            GoogleOrderId = @GoogleOrderId,
            PurchaseToken = @PurchaseToken,
            ExpiryDate    = @ExpiryDate,
            AutoRenew     = @AutoRenew,
            UpdatedAt     = GETUTCDATE()
        WHERE ProfileId = @ProfileId AND Status = 'Active';
    END
    ELSE
    BEGIN
        INSERT INTO UserSubscriptions
            (ProfileId, PlanId, GoogleOrderId, PurchaseToken,
             Status, StartDate, ExpiryDate, AutoRenew,
             CreatedAt, UpdatedAt)
        VALUES
            (@ProfileId, @PlanId, @GoogleOrderId, @PurchaseToken,
             @Status, @StartDate, @ExpiryDate, @AutoRenew,
             GETUTCDATE(), GETUTCDATE());
    END
END;
GO

-- SP 5: Update status (used by Google webhook for renewals/cancels/expiry)
CREATE OR ALTER PROCEDURE usp_UpdateSubscriptionStatus
    @PurchaseToken NVARCHAR(MAX),
    @Status        NVARCHAR(20),
    @ExpiryDate    DATETIME2
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE UserSubscriptions SET
        Status    = @Status,
        ExpiryDate = @ExpiryDate,
        UpdatedAt  = GETUTCDATE()
    WHERE PurchaseToken = @PurchaseToken;
END;
GO