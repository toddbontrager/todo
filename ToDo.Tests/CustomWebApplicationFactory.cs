using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc.Testing;
using ToDoApp;

namespace ToDo.Tests
{
    class CustomWebApplicationFactory : WebApplicationFactory<Startup>
    {
    }
}
