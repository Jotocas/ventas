namespace Ventas.Helpers
{
    using Plugin.Settings;
    using Plugin.Settings.Abstractions;

    public static class Settings
    {
        static ISettings AppSettings
        {
            get
            {
                return CrossSettings.Current;
            }
        }

        private const string tokenType = "TokenType";
        private const string accessToken = "AccessToken";
        private const string isRemenbered = "IsRemenbered";
        private static readonly string stringDefault = string.Empty;
        private static readonly bool booleanDefault = false;

        public static string TokenType
        {
            get
            {
                return AppSettings.GetValueOrDefault(tokenType, stringDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue(tokenType, value);
            }
        }


        public static string AccessToken
        {
            get
            {
                return AppSettings.GetValueOrDefault(accessToken, stringDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue(accessToken, value);
            }
        }

        public static bool IsRemenbered
        {
            get
            {
                return AppSettings.GetValueOrDefault(isRemenbered, booleanDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue(isRemenbered, value);
            }
        }
    }

}
