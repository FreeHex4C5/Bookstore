using DddBookstore.After;
using DddBookstore.Before;

if (args.FirstOrDefault()?.ToLowerInvariant() == "--before")
    BeforeDemo.Run();
else
    AfterDemo.Run();
