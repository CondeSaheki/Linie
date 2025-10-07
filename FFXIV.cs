
namespace Linie;

public static class FFXIV
{
    public static Job[] Jobs =>
    [
        Job.Paladin, Job.Warrior, Job.DarkKnight, Job.Gunbreaker,
        Job.WhiteMage,Job.Scholar,Job.Astrologian,Job.Sage,
        Job.Dragoon, Job.Monk, Job.Ninja, Job.Samurai, Job.Reaper, Job.Viper,
        Job.Bard, Job.Machinist, Job.Dancer,
        Job.BlackMage, Job.Summoner, Job.RedMage, Job.Pictomancer,
    ];

    public enum Job
    {
        Paladin, Warrior, DarkKnight, Gunbreaker,
        WhiteMage, Scholar, Astrologian, Sage,
        Dragoon, Monk, Ninja, Samurai, Reaper, Viper,
        Bard, Machinist, Dancer,
        BlackMage, Summoner, RedMage, Pictomancer,
    }

    public static string JobName(Job job) => job switch
    {
        Job.Paladin => "Paladin",
        Job.Warrior => "Warrior",
        Job.DarkKnight => "Dark Knight",
        Job.Gunbreaker => "Gunbreaker",
        Job.WhiteMage => "White Mage",
        Job.Scholar => "Scholar",
        Job.Astrologian => "Astrologian",
        Job.Sage => "Sage",
        Job.Dragoon => "Dragoon",
        Job.Monk => "Monk",
        Job.Ninja => "Ninja",
        Job.Samurai => "Samurai",
        Job.Reaper => "Reaper",
        Job.Viper => "Viper",
        Job.Bard => "Bard",
        Job.Machinist => "Machinist",
        Job.Dancer => "Dancer",
        Job.BlackMage => "Black Mage",
        Job.Summoner => "Summoner",
        Job.RedMage => "RedMage",
        Job.Pictomancer => "Pictomancer",
        _ => throw new NotImplementedException(),
    };
    
    public static string JobAbreviation(Job job) => job switch
    {
        Job.Paladin => "PLD",
        Job.Warrior => "WAR",
        Job.DarkKnight => "DRK",
        Job.Gunbreaker => "GNB",
        Job.WhiteMage => "WHM",
        Job.Scholar => "SCH",
        Job.Astrologian => "AST",
        Job.Sage => "SGE",
        Job.Dragoon => "DRG",
        Job.Monk => "MNK",
        Job.Ninja => "NIN",
        Job.Samurai => "SAM",
        Job.Reaper => "RPR",
        Job.Viper => "VPR",
        Job.Bard => "BRD",
        Job.Machinist => "MCH",
        Job.Dancer => "DNC",
        Job.BlackMage => "BLM",
        Job.Summoner => "SMN",
        Job.RedMage => "RDM",
        Job.Pictomancer => "PCT",
        _ => throw new NotImplementedException(),
    };
}