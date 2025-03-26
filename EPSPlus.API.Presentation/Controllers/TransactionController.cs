using EPSPlus.Application.Implementation;
using EPSPlus.Application.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace EPSPlus.API.Presentation.Controllers;

//[ApiController]
////[Authorize]
//[Route("api/[controller]")]
public class TransactionController : Controller
{

    private readonly ITransactionService _transactionService;

    public TransactionController(ITransactionService transactionService)
    {
        _transactionService = transactionService;
    }

    public IActionResult Index()
    {
        return View();
    }
}
