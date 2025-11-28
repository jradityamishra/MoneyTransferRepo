using AccountMicroservices.Data.Model;
using AccountMicroservices.Data.Service;
using AccountMicroservices.Data.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AccountMicroservices.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    //[Authorize(Roles = "Admin")]
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
        [HttpGet("{accountNumber}")]
        public async Task<ActionResult<Account>> GetById(string accountNumber)
        {
            var acc = await _accountService.GetByIdAsync(accountNumber);
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
            return Ok(new
            {
                message = "Account created successfully!",
                account = created
            });
            //return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        // PUT: api/account/5
        [HttpPut("{accountNumber}")]
        public async Task<IActionResult> Update(string accountNumber, [FromBody] UpdateAccountVM req)
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

            var success = await _accountService.UpdateAsync(accountNumber, updatedAccount);
            if (!success) return NotFound();
            return Ok("Account Updated");
        }

        [HttpPut("update-balance/x  {accountNumber}")]
        public async Task<IActionResult> UpdateBalance(string accountNumber, [FromBody] ChangeBalanceVM req)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var success = await _accountService.UpdateBalanceAsync(accountNumber, req);
            if (!success) return NotFound();
            return Ok("Balance Updated Successfully!");
        }


        // DELETE: api/account/5
        [HttpDelete("{accountNumber}")]
        public async Task<IActionResult> Delete(string accountNumber)
        {
            var success = await _accountService.DeleteAsync(accountNumber);
            if (!success) return NotFound();
            return Ok("Account Deleted Successfully!");
        }
    }
}
