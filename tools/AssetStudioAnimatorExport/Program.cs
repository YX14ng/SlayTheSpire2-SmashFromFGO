using AssetStudio;

if (args.Length is < 2 or > 3)
{
    Console.Error.WriteLine("Uso: AssetStudioAnimatorExport <bundle> <carpeta-salida> [animator=chr]");
    return 2;
}

var bundlePath = Path.GetFullPath(args[0]);
var outputDir = Path.GetFullPath(args[1]);
var wantedAnimator = args.Length == 3 ? args[2] : "chr";
if (!File.Exists(bundlePath))
{
    Console.Error.WriteLine($"No existe el bundle: {bundlePath}");
    return 2;
}

var manager = new AssetsManager();
try
{
    // El CLI oficial 0.19 omite AnimationClip de su filtro en modo Animator aun cuando
    // --fbx-animation=all esta activo. Cargar explicitamente ambos tipos reproduce la
    // seleccion de la GUI: Animator 'chr' + todos los AnimationClip del mismo bundle.
    manager.SetAssetFilter(
        ClassIDType.Animator,
        ClassIDType.AnimationClip,
        ClassIDType.Mesh,
        ClassIDType.Texture2D);
    manager.LoadFilesAndFolders(bundlePath);

    var objects = manager.AssetsFileList.SelectMany(file => file.Objects).ToList();
    var clips = objects.OfType<AnimationClip>().ToList();
    var animators = objects.OfType<Animator>().ToList();
    var namedAnimators = animators
        .Select(animator => (Animator: animator, Name: GetAnimatorName(animator)))
        .ToList();
    var selected = namedAnimators.FirstOrDefault(item =>
        string.Equals(item.Name, wantedAnimator, StringComparison.OrdinalIgnoreCase));

    if (selected.Animator is null)
    {
        Console.Error.WriteLine(
            $"No se encontro Animator '{wantedAnimator}'. Disponibles: " +
            string.Join(", ", namedAnimators.Select(item => item.Name)));
        return 3;
    }
    if (clips.Count == 0)
    {
        Console.Error.WriteLine("El bundle no contiene AnimationClip exportables.");
        return 4;
    }

    Console.WriteLine($"Animator: {selected.Name}");
    Console.WriteLine($"Clips: {clips.Count} ({string.Join(", ", clips.Select(clip => clip.m_Name))})");

    var converter = new ModelConverter(selected.Animator, ImageFormat.Png, clips);
    if (converter.AnimationList.Count == 0)
    {
        Console.Error.WriteLine("AssetStudio no pudo convertir ningun clip para este rig.");
        return 5;
    }

    var animatorDir = Path.Combine(outputDir, "Animator", selected.Name);
    var fbxPath = Path.Combine(animatorDir, selected.Name + ".fbx");
    Directory.CreateDirectory(animatorDir);
    ModelExporter.ExportFbx(fbxPath, converter, new Fbx.Settings { ExportAnimations = true });

    var size = new FileInfo(fbxPath).Length;
    Console.WriteLine($"[OK] {converter.AnimationList.Count} animaciones -> {fbxPath} ({size / 1024:N0} KB)");
    return 0;
}
catch (Exception exception)
{
    Console.Error.WriteLine(exception);
    return 1;
}
finally
{
    manager.Clear();
}

static string GetAnimatorName(Animator animator)
{
    return animator.m_GameObject.TryGet(out var gameObject)
        ? gameObject.m_Name
        : $"Animator_{animator.m_PathID}";
}
