using System.Linq;
using MoveLikeJogger.DataContracts.Identity;
using MoveLikeJogger.DataContracts.Moves;
using MoveLikeJogger.DataMining.Commands;
using MoveLikeJogger.DataMining.Commands.Identity;
using MoveLikeJogger.DataMining.Commands.Moves;
using MoveLikeJogger.DataMining.DB;
using MoveLikeJogger.DataMining.Queries;
using MoveLikeJogger.DataMining.Queries.Identity;
using MoveLikeJogger.DataMining.Queries.Moves;
using MoveLikeJogger.DataModels.Moves;
using StructureMap;

namespace MoveLikeJogger.DependencyResolution
{
    public static class IoC
    {
        public static IContainer Initialize()
        {
            System.Diagnostics.Debug.WriteLine("Creating and initializing DI IOC Container...", "DI IOC");

            var container = new Container(x =>
            {
                x.Scan(scan =>
                {
                    scan.TheCallingAssembly();
                    //scan.LookForRegistries();
                    scan.WithDefaultConventions();
                });

                RegisterAdditionaly(x);
            });

            System.Diagnostics.Debug.WriteLine("Creating and initializing DI IOC Container... DONE!", "DI IOC");

            return container;
        }

        private static void RegisterAdditionaly(ConfigurationExpression cfg)
        {
            cfg.For<IQuery<IQueryable<ApplicationUserDTO>, bool>>().Use<UserInfoQuery>();
            cfg.For<IDeleteUserCommand>().Use<DeleteUserCommand>();

            cfg.For<IQuery<IQueryable<MoveDTO>, bool>>().Use<MovesQuery>();

            cfg.For<IDeleteMoveCommand>().Use<DeleteMoveCommand>();
            cfg.For<ICommand<bool, Move>>().Use<SaveMoveCommand>();

            cfg.For<IApplicationDbContext>().Use<ApplicationDbContext>();
        }
    }
}