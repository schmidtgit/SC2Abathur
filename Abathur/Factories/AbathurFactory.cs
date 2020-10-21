using Abathur.Core;
using Abathur.Core.Combat;
using Abathur.Core.Intel;
using Abathur.Core.Intel.Map;
using Abathur.Core.Production;
using Abathur.Core.Raw;
using Abathur.Modules;
using Abathur.Repositories;
using Microsoft.Extensions.DependencyInjection;
using NydusNetwork;
using NydusNetwork.API.Protocol;
using NydusNetwork.Logging;
using NydusNetwork.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Abathur.Factory {
    public class AbathurFactory {
        private ILogger log;

        public AbathurFactory(ILogger log = null) => this.log = log;

        public IAbathur Create(
          GameSettings gameSettings,
          Essence essence,
          ILogger logger,
          Assembly assembly,
          params string[] modules) {
            List<Type> typeList = new List<Type>();
            Queue<string> stringQueue = new Queue<string>();
            foreach (string module in modules) {
                Type type;
                if (GetType<IModule>(assembly, module, out type)) {
                    typeList.Add(type);
                    log?.LogSuccess((object)string.Format("AbathurFactory: {0} resolved to a valid class.", (object)module));
                } else {
                    log?.LogWarning((object)string.Format("AbathurFactory: {0} could not be resolved!", (object)module));
                }
            }
            Type[] array = GetTypes<IReplaceableModule>(assembly).ToArray<System.Type>();
            Abathur abathur = (Abathur)this.Create(gameSettings, essence, logger, (IEnumerable<System.Type>)typeList, array);
            return abathur;
        }

        public IAbathur Create(
          GameSettings gameSettings,
          Essence essence,
          ILogger logger,
          IEnumerable<Type> modules,
          params Type[] types) {
            IServiceProvider provider = AbathurFactory.ConfigureServices(gameSettings, essence, logger, modules, types);
            Abathur service = (Abathur)provider.GetService<IAbathur>();
            service.Modules = provider.GetServices<IModule>().ToList<IModule>();
            return service;
        }

        private static IEnumerable<System.Type> GetTypes<T>(Assembly assembly) {
            TypeInfo info = typeof(T).GetTypeInfo();
            return ((IEnumerable<System.Type>)info.Assembly.GetTypes()).Concat<System.Type>((IEnumerable<System.Type>)assembly.GetTypes()).Where<System.Type>((Func<System.Type, bool>)(x => x != typeof(T))).Where<System.Type>((Func<System.Type, bool>)(x => info.IsAssignableFrom(x)));
        }

        private static bool GetType<T>(Assembly assembly, string classname, out Type type) {
            TypeInfo info = typeof(T).GetTypeInfo();
            type = ((IEnumerable<Type>)info.Assembly.GetTypes()).Concat<Type>((IEnumerable<System.Type>)assembly.GetTypes()).Where<System.Type>((Func<System.Type, bool>)(x => x != typeof(T))).Where<System.Type>((Func<System.Type, bool>)(x => info.IsAssignableFrom(x))).Where<System.Type>((Func<System.Type, bool>)(x => x.Name == classname)).FirstOrDefault<System.Type>();
            return !(type == (Type)null);
        }

        private static IServiceProvider ConfigureServices(
          GameSettings gameSettings,
          Essence essence,
          ILogger log,
          IEnumerable<System.Type> modules,
          params System.Type[] types) {
            ServiceCollection services = new ServiceCollection();
            services.AddSingleton<Essence>(essence);
            services.AddSingleton<GameSettings>(gameSettings);
            services.AddSingleton<ILogger>(log);
            services.AddSingleton<IGameClient, GameClient>();
            services.AddSingleton<IIntelManager, IntelManager>();
            services.AddSingleton<IRawManager, RawManager>();
            services.AddSingleton<IProductionManager, ProductionManager>();
            services.AddSingleton<ICombatManager, CombatManager>();
            services.AddSingleton<ISquadRepository, CombatManager>();
            services.AddSingleton<IGameMap, GameMap>();
            services.AddSingleton<ITechTree, TechTree>();
            services.AddSingleton<DataRepository, DataRepository>();
            services.AddSingleton<IUnitTypeRepository>((Func<IServiceProvider, IUnitTypeRepository>)(x => (IUnitTypeRepository)x.GetService<DataRepository>()));
            services.AddSingleton<IUpgradeRepository>((Func<IServiceProvider, IUpgradeRepository>)(x => (IUpgradeRepository)x.GetService<DataRepository>()));
            services.AddSingleton<IBuffRepository>((Func<IServiceProvider, IBuffRepository>)(x => (IBuffRepository)x.GetService<DataRepository>()));
            services.AddSingleton<IAbilityRepository>((Func<IServiceProvider, IAbilityRepository>)(x => (IAbilityRepository)x.GetService<DataRepository>()));
            foreach (Type module in modules)
                ServiceCollectionServiceExtensions.AddSingleton(services, typeof(IModule), module);
            foreach (Type type in types)
                services.AddScoped(type, type);
            services.AddSingleton<IAbathur, Abathur>();
            return (IServiceProvider)services.BuildServiceProvider();
        }
    }
}
