using MoveLikeJogger;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(StructureMapConfigurator), "Configure", Order = 1)]
