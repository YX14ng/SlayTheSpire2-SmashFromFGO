using Godot;

namespace AstolfoRider.AstolfoRiderCode.Extensions;

public static class StringExtensions
{
    public static string CardImagePath(this string path) => Path.Join(MainFile.ResPath, "images", "card_portraits", path);
    public static string BigCardImagePath(this string path) => Path.Join(MainFile.ResPath, "images", "card_portraits", "big", path);
    public static string PowerImagePath(this string path) => Path.Join(MainFile.ResPath, "images", "powers", path);
    public static string BigPowerImagePath(this string path) => Path.Join(MainFile.ResPath, "images", "powers", "big", path);
    public static string RelicImagePath(this string path) => Path.Join(MainFile.ResPath, "images", "relics", path);
    public static string BigRelicImagePath(this string path) => Path.Join(MainFile.ResPath, "images", "relics", "big", path);
    public static string CharacterUiPath(this string path) => Path.Join(MainFile.ResPath, "images", "charui", path);
}
