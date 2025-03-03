using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plant_Explorer.Contract.Repositories.ModelViews.AuthModel
{
    public class GoogleLoginResponse
    {
        public string Token { get; set; }
        public string UserId { get; set; }
    }
}
