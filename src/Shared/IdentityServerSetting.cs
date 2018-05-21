using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public class IdentityServerSetting
    {
        public const string BASE_IDENTITY_SERVER_URL = "https://localhost:44388/identity";
        public const string IDENTITY_SERVER_TOKEN_URL = BASE_IDENTITY_SERVER_URL + "/connect/token";

        public const string MVC_CLIENT_CLIENT_ID = "webapp";
        public const string MVC_CLIENT_CLIENT_SECRET = "secret";
        public const string MVC_CLIENT_REDIRECT_URI = "https://localhost:44357/";
        public const string MVC_CLIENT_POST_REDIRECT_URI = "https://localhost:44357/";

        public const string WEB_API_CLIENT_ID = "webapi";
        public const string WEB_API_CLIENT_SECRET = "secret";
        public const string WEB_API_RESOURCE_URL = "https://localhost:44383/api/WebApiResrouce";

        public const string ANGULAR_CLIENT_ID = "angular";
        public const string ANGULAR_CLIENT_REDIRECT_URI = "https://localhost:4200/";
        public const string ANGULAR_CLIENT_POST_REDIRECT_URI = "https://localhost:4200/";
        public const string ANGULAR_CLIENT_SILENT_REFRESH_REDIRECT_URI = "https://localhost:4200/silent-refresh.html";

        public const string WEBFORM_CLIENT_ID = "webform";
        public const string WEBFORM_CLIENT_SECRET = "secrte";
        public const string WEBFORM_REDIRECT_URI = "https://localhost:44346/";
        public const string WEBFORM_POST_REDIRECT_URI = "https://localhost:44346/";

        public const string IDENTITY_SERVER_CONNECTION_STRING = "IdentityServerConnectionString";
    }
}
