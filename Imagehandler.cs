namespace ImgPad;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Jpeg;
using System.Drawing;

public static class ImageHandler
{
    public static void ProcessarPasta(string caminhoPasta, Action<string, System.Drawing.Color?> log, Action<int, int> progresso)
    {
        List<string> imagens = GetImagensPasta(caminhoPasta);

        var outputFolder = caminhoPasta + "/tratadas";
        Directory.CreateDirectory(outputFolder);

        log($"Processando... ({imagens.Count} arquivos encontrados)", null);

        for (int i = 0; i < imagens.Count; i++)
        {
            string imagem = imagens[i];
            try
            {
                var nome = Path.GetFileNameWithoutExtension(imagem) + ".jpg";
                var outputPath = Path.Combine(outputFolder, nome);
                var result = ProcessarImagem(imagem, outputPath);
                log($"OK: {Path.GetFileName(imagem)} — {result.SizeKb} KB (q{result.Quality})", System.Drawing.Color.FromArgb(100, 220, 130));
            }
            catch
            {
                log($"ERRO: {imagem}", System.Drawing.Color.FromArgb(220, 80, 80));
            }

            progresso(i + 1, imagens.Count);
        }

        log("Concluído.", System.Drawing.Color.FromArgb(99, 102, 241));
    }

    private static (double SizeKb, int Quality) ProcessarImagem(string inputPath, string outputPath, int maxDimension = 2000, int maxSizeKb = 350)
    {
        using var img = SixLabors.ImageSharp.Image.Load(inputPath);

        img.Mutate(x => x.AutoOrient());

        // Resize mantendo proporção
        img.Mutate(x => x.Resize(new ResizeOptions
        {
            Size = new SixLabors.ImageSharp.Size(maxDimension, maxDimension * 3 / 4),
            Mode = ResizeMode.Pad,
            PadColor = SixLabors.ImageSharp.Color.White
        }));
        var quality = 90;
        double sizeKb;

        while (true)
        {
            var encoder = new JpegEncoder { Quality = quality };
            img.Save(outputPath, encoder);
            sizeKb = new FileInfo(outputPath).Length / 1024.0;

            if (sizeKb <= maxSizeKb) break;

            quality = quality <= 30 ? quality - 1 : quality - 5;

            if (quality <= 5) break;
        }

        return (Math.Round(sizeKb, 2), quality);
    }

    private static List<string> GetImagensPasta(string caminhoPasta)
    {
        var extensoes = new[] { "*.jpg", "*.jpeg", "*.png", "*.webp" };
        return extensoes.SelectMany(ext => Directory.GetFiles(caminhoPasta, ext)).ToList();
    }
}