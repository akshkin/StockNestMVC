using Supabase.Storage;

namespace StockNestMVC.DTOs;

public class UploadResponseDto
{
    public UploadSignedUrl SignedUrl {  get; set; }

    public string   FilePath { get; set; }
        
    public string   Bucket {  get; set; }
}
