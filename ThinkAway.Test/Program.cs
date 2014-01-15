using System.Windows.Forms;
namespace ThinkAway.Test
{
    class Program
    {
        static void Main()
        {
            Application.ThreadException += Application_ThreadException;
            TEST test = new TEST();
            test.SocketTest();
            System.Console.ReadKey(false);
        }

        static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            System.Console.WriteLine(e.Exception.Message);
            System.Console.ReadKey(true);
        }
    }
}
