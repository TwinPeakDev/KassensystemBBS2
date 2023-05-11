using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kassensystem.Data;

public class ProductImage
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public ProductImage(string imageName)
    {
        ImageName = imageName;
    }

    private string _imageName;

    public string ImageName
    {
        get => _imageName;
        set
        {
            _imageName = value;
            InitializeImage();
        }
    }

    public (int, int) WidthAndHeight { get; private set; }
    public (int, int) AspectRatio { get; private set; }

    private void InitializeImage()
    {
        CalculateWidthAndHeight();
        CalculateRatio();
    }
    
    public string GetLocalImageBase64()
    {
        var imageFolder = @"\wwwroot\Uploads";
        var uploadPath = Environment.CurrentDirectory + imageFolder;
        if (ImageName == null) return "";
        
        var fullPath = Path.Combine(uploadPath,  ImageName);

        return !File.Exists(fullPath) ? "" : Convert.ToBase64String(File.ReadAllBytes(fullPath));
    }


    private void CalculateWidthAndHeight()
    {
        var imageBase64 = GetLocalImageBase64();

        if (imageBase64 == "")
        {
            WidthAndHeight = (0, 0);
            return;
        }
        
        var image = Image.Load(new MemoryStream(Convert.FromBase64String(imageBase64)));

        WidthAndHeight = (image.Width, image.Height);
    }

    private void CalculateRatio()
    {
        var startTime = DateTime.Now;
        if (WidthAndHeight.Item1 == 0 || WidthAndHeight.Item2 == 0)
        {
            AspectRatio = (0, 0);
            return;
        }

        int larger, smaller;
        
        if (WidthAndHeight.Item1 > WidthAndHeight.Item2)
        {
            larger = WidthAndHeight.Item1;
            smaller = WidthAndHeight.Item2;
        }
        else
        {
            larger = WidthAndHeight.Item2;
            smaller = WidthAndHeight.Item1;
        }
        
        //get greatest common divisor using euclidean algorithm
        
        double remainderLarge = larger % smaller;
        double remainderSmall = smaller % remainderLarge;

        double last = 0;
        
        while (remainderSmall != 0)
        {
            last = remainderSmall;
            remainderSmall = remainderLarge % remainderSmall;
            remainderLarge = remainderSmall;
        }

        
        Console.WriteLine((DateTime.Now - startTime).Milliseconds);
        AspectRatio = (WidthAndHeight.Item1 / (int)last, WidthAndHeight.Item2 / (int)last);
    }
    

}