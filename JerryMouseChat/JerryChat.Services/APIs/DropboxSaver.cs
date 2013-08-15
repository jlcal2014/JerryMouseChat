using System;
using System.IO;
using System.Diagnostics;
using Spring.Social.OAuth1;
using Spring.Social.Dropbox.Api;
using Spring.Social.Dropbox.Connect;
using Spring.IO;
using System.Threading;

public class DropboxSaver
{
	// Register your own Dropbox app at https://www.dropbox.com/developers/apps
	// with "Full Dropbox" access level and set your app keys and app secret below
    private const string DropboxAppKey = "tjxmn816e4q5f4n";
    private const string DropboxAppSecret = "97xdegylbq5qvh0";

	private const string OAuthTokenFileName = "C:\\TestFiles\\OAuthTokenFileName.txt";

    public static string DropboxInit(string filePath)
    {
        DropboxServiceProvider dropboxServiceProvider =
                  new DropboxServiceProvider(DropboxAppKey, DropboxAppSecret, AccessLevel.AppFolder);


        // Authenticate the application (if not authenticated) and load the OAuth token
        if (!File.Exists(OAuthTokenFileName))
        {
            AuthorizeAppOAuth(dropboxServiceProvider);
        }
        OAuthToken oauthAccessToken = LoadOAuthToken();

        // Login in Dropbox
        IDropbox dropbox = dropboxServiceProvider.GetApi(oauthAccessToken.Value, oauthAccessToken.Secret);

        //get the file name
        var fileUrl = filePath.Split('/', '\\');
        int lastIndex = fileUrl.Length;
        string fileName = fileUrl[lastIndex - 1];

        // Create new folder
        string newFolderName = "New_Folder_" + DateTime.Now.Ticks;
        Entry createFolderEntry = dropbox.CreateFolderAsync(newFolderName).Result;

        //add the file name to the folder
        newFolderName += "/" + fileName;
        string uploadFileEntry = DropboxUpload(filePath, dropbox, newFolderName);

        // Share a file
        return uploadFileEntry;
    }

    private static string DropboxUpload(string fileUrl, IDropbox dropbox, string newFolderName)
    {
        Entry uploadFileEntry = dropbox.UploadFileAsync(
            new FileResource(fileUrl),
            "/" + newFolderName).Result;
        DropboxLink sharedUrl = dropbox.GetShareableLinkAsync(uploadFileEntry.Path).Result;

        return sharedUrl.Url;
    }
  
	private static OAuthToken LoadOAuthToken()
	{
		string[] lines = File.ReadAllLines(OAuthTokenFileName);
		OAuthToken oauthAccessToken = new OAuthToken(lines[0], lines[1]);
		return oauthAccessToken;
	}
  
	private static void AuthorizeAppOAuth(DropboxServiceProvider dropboxServiceProvider)
	{

        // Authorization without callback url
        Console.Write("Getting request token...");
        OAuthToken oauthToken = dropboxServiceProvider.OAuthOperations.FetchRequestTokenAsync(null, null).Result;
        Console.WriteLine("Done.");

        OAuth1Parameters parameters = new OAuth1Parameters();
        string authenticateUrl = dropboxServiceProvider.OAuthOperations.BuildAuthorizeUrl(
            oauthToken.Value, parameters);
        Console.WriteLine("Redirect the user for authorization to {0}", authenticateUrl);
        Process.Start(authenticateUrl);
        Console.Write("Press [Enter] when authorization attempt has succeeded.");
        //Console.ReadLine();
        Thread.Sleep(10000);
        Console.Write("Getting access token...");
        AuthorizedRequestToken requestToken = new AuthorizedRequestToken(oauthToken, null);
        OAuthToken oauthAccessToken =
            dropboxServiceProvider.OAuthOperations.ExchangeForAccessTokenAsync(requestToken, null).Result;
        Console.WriteLine("Done.");

        string[] oauthData = new string[] { oauthAccessToken.Value, oauthAccessToken.Secret };
        File.WriteAllLines(OAuthTokenFileName, oauthData);
	}
}
