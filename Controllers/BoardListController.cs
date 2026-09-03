using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TaskTracker.Data;
using TaskTracker.Models;

namespace TaskTracker.Controllers
{
    [Authorize]
    public class BoardListController : Controller
    {
        private readonly AppDbContext _context;

        public BoardListController(AppDbContext context)
        {
            _context = context;
        }

        private int GetUserId()
        {
            return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Criar(int boardId, string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return RedirectToAction("Detalhes", "Board", new { id = boardId });
            }

            var userId = GetUserId();

            var board = await _context.Boards
                .FirstOrDefaultAsync(b =>
                    b.Id == boardId &&
                    b.OwnerId == userId);

            if (board == null)
            {
                return NotFound();
            }

            var lastPosition = await _context.BoardLists
                .Where(l => l.BoardId == boardId)
                .Select(l => (int?)l.Position)
                .MaxAsync() ?? -1;

            var list = new BoardList
            {
                Name = name.Trim(),
                BoardId = boardId,
                Position = lastPosition + 1
            };

            _context.BoardLists.Add(list);

            await _context.SaveChangesAsync();

            return RedirectToAction("Detalhes","Board",new { id = boardId });
        }
    }
}