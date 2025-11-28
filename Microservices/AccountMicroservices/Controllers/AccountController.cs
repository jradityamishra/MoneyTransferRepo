using AccountMicroservices.Data.Model;
using AccountMicroservices.Data.Service;
using AccountMicroservices.Data.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AccountMicroservices.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    [Authorize(Roles = "Admin")]
    public class AccountController : ControllerBase
    {
        

        private readonly AccountService _accountService;

        public AccountController(AccountService svc)
        {
            _accountService = svc;
        }

        [HttpGet("test")]
        public IActionResult Account()
        {
            return Ok("Account Microservice is working!");
        }
        // GET: api/account
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Account>>> GetAll()
        {
            var accounts = await _accountService.GetAllAsync();
            return Ok(accounts);
        }

        // GET: api/account/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Account>> GetById(int id)
        {
            var acc = await _accountService.GetByIdAsync(id);
            if (acc == null) return NotFound();
            return Ok(acc);
        }

        // POST: api/account
        [HttpPost]
        public async Task<ActionResult<Account>> Create([FromBody] CreateAccountVM req)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var account = new Account
            {
                UserID = req.UserID,
                AccountNumber = req.AccountNumber,
                Balance = req.Balance,
                Currency = req.Currency,
                Status = req.Status
            };

            var created = await _accountService.CreateAsync(account);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        // PUT: api/account/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateAccountVM req)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var updatedAccount = new Account
            {
                UserID = req.UserID,
                AccountNumber = req.AccountNumber,
                Balance = req.Balance,
                Currency = req.Currency,
                Status = req.Status
            };

            var success = await _accountService.UpdateAsync(id, updatedAccount);
            if (!success) return NotFound();
            return NoContent();
        }

        // DELETE: api/account/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _accountService.DeleteAsync(id);
            if (!success) return NotFound();
            return NoContent();
        }
    }
}
