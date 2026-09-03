using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TaskTracker.Data;
using TaskTracker.Models;

namespace TaskTracker.Controllers
{
    [Authorize]
    public class BoardController : Controller
    {
        private readonly AppDbContext _context;

        public BoardController(AppDbContext context)
        {
            _context = context;
        }

        private int GetUserId()
        {
            return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        }

        public async Task<IActionResult> Index()
        {
            var userId = GetUserId();

            var boards = await _context.Boards
                .Where(b => b.OwnerId == userId)
                .ToListAsync();

            return View(boards);
        }
        public async Task<IActionResult> Detalhes(int id)
        {
            var userId = GetUserId();

            var board = await _context.Boards
                .Include(b => b.Lists)
                .ThenInclude(l => l.Tarefas)
                .ThenInclude(t => t.Categoria)
                .FirstOrDefaultAsync(b =>
                    b.Id == id &&
                    b.OwnerId == userId);

            if (board == null)
            {
                return NotFound();
            }

            return View(board);
        }

        public IActionResult Criar()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Criar(Board board)
        {
            if (!ModelState.IsValid)
            {
                return View(board);
            }

            board.OwnerId = GetUserId();
            board.CreatedAt = DateTime.UtcNow;

            _context.Boards.Add(board);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}