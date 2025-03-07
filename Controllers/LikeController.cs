namespace Babel.Controllers;
using Babel.Services.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class LikeController : ControllerBase
{
    private readonly ILikeService _likeService;

    public LikeController(ILikeService likeService)
    {
        _likeService = likeService;
    }

    [HttpPost("toggle/{libroId}")]
    [Authorize]
    public async Task<IActionResult> ToggleLike(int libroId, string token)
    {
        try
        {
            bool liked = await _likeService.ToggleLikeByUserAsync(libroId, token);
            return Ok(new { liked, message = liked ? "Like agregado" : "Like eliminado" });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
    [HttpGet("likes-info/{libroId}")]
    public async Task<IActionResult> GetLibroLikesInfo(int libroId)
    {
        try
        {
            var likeInfo = await _likeService.GetLibroLikesInfoAsync(libroId);
            return Ok(likeInfo);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
    [Authorize(Roles ="Admin")]
    [HttpGet("all-likes-info")]
    public async Task<IActionResult> GetAllLikesInfo()
    {
        try
        {
            var allLikesInfo = await _likeService.GetAllLikesInfoAsync();
            return Ok(allLikesInfo);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
    [HttpGet("usuario")]
    public async Task<IActionResult> GetLikesByUser(string token)
    {
        var likedBooks = await _likeService.GetLikesByUserAsync(token);
        return Ok(likedBooks);
    }
    [Authorize(Roles = "Admin")]
    [HttpDelete("delete-all")]
    public async Task<IActionResult> DeleteAllLikes()
    {
        try
        {
            var result = await _likeService.DeleteAllLikesAsync();
            if (!result) return NotFound(new { message = "No hay likes para eliminar." });

            return Ok(new { message = "Todos los likes han sido eliminados." });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }


}

