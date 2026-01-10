namespace Media.Core
{
    /// <summary>
    /// Standardized error messages for the users.
    /// </summary>
    public static class ErrorMessages
    {

        #region Token Errors
        public static string TokenNameAlreadyUsed(string tokenName) => 
            $"Token name '{tokenName}' is already used.";

        public static string TokenDoesNotExist(string token) => 
            $"Token '{token}' does not exist.";

        public static string CannotDeactivateToken(string token) =>
            $"Cannot deactivate token '{token}'. Token may already be inactive or does not exist.";

        public static string CannotUpdateTokenPermissions() =>
            $"Cannot update token permissions.";

        #endregion
        #region Auth Errors
        public static string TokenDoesNotHavePermissions() =>
            $"The provided token does not have the required permissions.";

        public static string NoAuthTokenInHeader() =>
            $"No authorization token provided in header.";

        public static string CannotUseTokenItIsNoun(string noun) =>
            $"Cannot use this token. Token is {noun}.";

        #endregion
        #region Media Errors
        public static string MediaItemNotFound() =>
            $"Media item not found.";

        public static string CouldNotActionMediaMissingPermission(string action, string permission) =>
            $"Cannot {action} this media item. The provided token lacks the {permission} permission.";

        public static string CouldNotActionMediaUserIsNotOwner(string action) =>
            $"Cannot {action} this media item. The provided token does not own this media item.";

        #endregion
        #region File Errors
        public static string FileTooLarge(long limit) =>
            $"File size exceeds the maximum limit of {limit} bytes.";
        
        public static string FileExtensionNotAllowed(string extension) =>
            $"Files with the extension '{extension}' are not allowed.";

        public static string FileNullOrEmpty() =>
            $"Uploaded file is null or empty.";

        public static string AccessToFileDenied() =>
            $"Access to the file denied.";

        public static string FileNotFound() =>
            $"File not found.";

        #endregion

    }
}
