using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JohnsenArtAPI.Controllers;

[Authorize]
[Route("api/[controller]")]
public class AdminController : ControllerBase 
{
    
}