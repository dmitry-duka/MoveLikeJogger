using System;
using System.Web.OData.Builder;
using MoveLikeJogger.DataContracts.Identity;
using MoveLikeJogger.DataContracts.Moves;

namespace MoveLikeJogger.OData
{
    public static class ODataModelBuilderHelper
    {
        public static void BuildModel(ODataModelBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.ConfigureUnbound();

            // configure entity sets
            builder.EntitySet<ApplicationUserDTO>("users");
            builder.EntitySet<MoveDTO>("moves");
        }

        private static void ConfigureUnbound(this ODataModelBuilder builder)
        {
            builder.Function("identity").Returns<IdentityDTO>();
            builder.Action("logout");
            builder.Action("login").Parameter<LoginDTO>("data");
            builder.Action("register").Parameter<LoginRegisterDTO>("data");
            builder.Action("account").Parameter<LoginUpdateDTO>("data");

            builder.Function("statistics").Returns<MoveStatisticsDTO>();
        }
    }
}
