namespace UcarMobileApi.Tests.Unit.Payments
{
    /// <summary>
    /// Stripe test card constants for different scenarios
    /// Reference: https://stripe.com/docs/testing#cards
    /// </summary>
    public static class StripeTestCards
    {
        #region Successful Cards

        public static class Successful
        {
            public const string Visa = "pm_card_visa";
            public const string VisaDebit = "pm_card_visa_debit";
            public const string Mastercard = "pm_card_mastercard";
            public const string MastercardDebit = "pm_card_mastercard_debit";
            public const string MastercardPrepaid = "pm_card_mastercard_prepaid";
            public const string AmericanExpress = "pm_card_amex";
            public const string Discover = "pm_card_discover";
            public const string DinersClub = "pm_card_diners";
            public const string JCB = "pm_card_jcb";
            public const string UnionPay = "pm_card_unionpay";
        }

        #endregion

        #region Declined Cards

        public static class Declined
        {
            public const string Generic = "pm_card_chargeDeclined";
            public const string InsufficientFunds = "pm_card_chargeDeclinedInsufficientFunds";
            public const string LostCard = "pm_card_chargeDeclinedLostCard";
            public const string StolenCard = "pm_card_chargeDeclinedStolenCard";
            public const string ExpiredCard = "pm_card_expiredCard";
            public const string IncorrectCVC = "pm_card_cvcDecline";
            public const string ProcessingError = "pm_card_chargeDeclinedProcessingError";
            public const string IncorrectNumber = "pm_card_chargeDeclinedIncorrectNumber";
        }

        #endregion

        #region Authentication Required (3D Secure)

        public static class Authentication
        {
            public const string Required = "pm_card_authenticationRequired";
            public const string RequiredSetupForOffSession = "pm_card_authenticationRequiredSetupForOffSession";
        }

        #endregion

        #region Risk and Fraud

        public static class Risk
        {
            public const string ElevatedRisk = "pm_card_riskLevelElevated";
            public const string HighestRisk = "pm_card_riskLevelHighest";
        }

        #endregion

        #region International Cards

        public static class International
        {
            public const string Brazil = "pm_card_br";
            public const string Canada = "pm_card_ca";
            public const string Mexico = "pm_card_mx";
            public const string UnitedKingdom = "pm_card_gb";
            public const string Germany = "pm_card_de";
            public const string France = "pm_card_fr";
            public const string Japan = "pm_card_jp";
            public const string Australia = "pm_card_au";
        }

        #endregion

        #region Specific Scenarios

        public static class Scenarios
        {
            // Cards that require specific handling
            public const string AlwaysAuthenticate = "pm_card_authenticationRequired";
            public const string OnlinePin = "pm_card_authenticationRequiredOnlinePin";

            // Dispute scenarios
            public const string DisputeFraudulent = "pm_card_createDispute";
            public const string DisputeProductNotReceived = "pm_card_createDisputeProductNotReceived";
            public const string DisputeInquiry = "pm_card_createDisputeInquiry";

            // Refund scenarios
            public const string RefundFailure = "pm_card_refundFailure";
        }

        #endregion

        #region Test Card Numbers (for manual testing)

        public static class CardNumbers
        {
            // Successful cards
            public const string VisaSuccess = "4242424242424242";
            public const string VisaDebitSuccess = "4000056655665556";
            public const string MastercardSuccess = "5555555555554444";
            public const string AmexSuccess = "378282246310005";

            // Declined cards
            public const string GenericDecline = "4000000000000002";
            public const string InsufficientFunds = "4000000000009995";
            public const string LostCard = "4000000000009987";
            public const string StolenCard = "4000000000009979";
            public const string ExpiredCard = "4000000000000069";
            public const string IncorrectCVC = "4000000000000127";
            public const string ProcessingError = "4000000000000119";

            // 3D Secure required
            public const string AuthenticationRequired = "4000002500003155";
            public const string AuthenticationRequiredSetup = "4000002760003184";
        }

        #endregion

        #region Test Amounts (in cents)

        public static class TestAmounts
        {
            // Amounts that trigger specific behaviors
            public const long ChargeDeclined = 2; // Any amount ending in 02 will be declined
            public const long InsufficientFunds = 9995; // Triggers insufficient_funds error
            public const long LostCard = 9987; // Triggers lost_card error
            public const long StolenCard = 9979; // Triggers stolen_card error
            public const long ExpiredCard = 4; // Any amount ending in 04 triggers expired_card
            public const long IncorrectCVC = 127; // Any amount ending in 27 triggers incorrect_cvc
            public const long ProcessingError = 119; // Any amount ending in 19 triggers processing_error

            // Standard test amounts
            public const long SmallAmount = 1000; // $10.00
            public const long MediumAmount = 5000; // $50.00
            public const long LargeAmount = 10000; // $100.00
        }

        #endregion

        #region Currency Codes

        public static class Currencies
        {
            public const string USD = "usd";
            public const string EUR = "eur";
            public const string GBP = "gbp";
            public const string CAD = "cad";
            public const string AUD = "aud";
            public const string JPY = "jpy";
            public const string BRL = "brl";
            public const string MXN = "mxn";
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Gets the expected last 4 digits for a test payment method
        /// </summary>
        public static string GetLast4ForTestCard(string paymentMethodId)
        {
            return paymentMethodId switch
            {
                Successful.Visa => "4242",
                Successful.VisaDebit => "5556",
                Successful.Mastercard => "4444",
                Successful.AmericanExpress => "0005",
                Declined.Generic => "0002",
                Declined.InsufficientFunds => "9995",
                Declined.LostCard => "9987",
                Declined.StolenCard => "9979",
                Declined.ExpiredCard => "0069",
                Declined.IncorrectCVC => "0127",
                Declined.ProcessingError => "0119",
                Authentication.Required => "3155",
                _ => "0000"
            };
        }

        /// <summary>
        /// Gets the expected brand for a test payment method
        /// </summary>
        public static string GetBrandForTestCard(string paymentMethodId)
        {
            return paymentMethodId switch
            {
                _ when paymentMethodId.Contains("visa") => "visa",
                _ when paymentMethodId.Contains("mastercard") => "mastercard",
                _ when paymentMethodId.Contains("amex") => "amex",
                _ when paymentMethodId.Contains("discover") => "discover",
                _ when paymentMethodId.Contains("diners") => "diners",
                _ when paymentMethodId.Contains("jcb") => "jcb",
                _ when paymentMethodId.Contains("unionpay") => "unionpay",
                _ => "visa" // Default to visa for generic test cards
            };
        }

        #endregion
    }
}
