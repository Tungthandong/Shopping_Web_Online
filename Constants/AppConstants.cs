namespace Shopping_Web.Constants
{
    public static class AppConstants
    {
        // Account statuses
        public const string StatusActive   = "active";
        public const string StatusInactive = "inactive";
        public const string StatusBanned   = "banned";

        // Roles
        public const string RoleAdmin    = "admin";
        public const string RoleCustomer = "customer";

        // Order statuses
        public const string OrderPending   = "Pending";
        public const string OrderConfirmed = "Confirmed";
        public const string OrderShipping  = "Shipping";
        public const string OrderCompleted = "Completed";
        public const string OrderCancelled = "Cancelled";

        // Session keys
        public const string SessionUsername = "Username";
        public const string SessionRole     = "Role";
        public const string SessionFullname = "Fullname";
        public const string SessionAvatar   = "Avatar";

        // Payment methods
        public const string PaymentCod   = "COD";
        public const string PaymentVnPay = "VnPay";
    }
}
