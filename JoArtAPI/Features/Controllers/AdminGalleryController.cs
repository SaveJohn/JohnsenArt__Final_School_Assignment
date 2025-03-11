using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JohnsenArtAPI.Controllers;

[Authorize]
[Route("admin/api/[controller]")]
public class AdminGalleryController : ControllerBase 
{
    
}