using MelonLoader;

[assembly: MelonInfo(typeof(Config.Core), "Config", "1.0.0", "po0po", null)]
[assembly: MelonGame("Vedinad", "Megabonk Demo")]

namespace Config
{
    public class Core : MelonMod
    {
        public override void OnInitializeMelon()
        {
            LoggerInstance.Msg("Initialized.");
        }
    }
}