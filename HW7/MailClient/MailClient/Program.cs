using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class Program
{
    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;

        string email = "dudok405@gmail.com";
        string password = "";

        ImapClient client = new ImapClient();

        try
        {
            client.Connect("imap.gmail.com", 993, true);
            client.Authenticate(email, password);

            Console.WriteLine("Connected!\n");

            bool running = true;

            while (running)
            {
                Console.WriteLine("\n===== MENU =====");
                Console.WriteLine("1 - Show folders");
                Console.WriteLine("2 - Show messages");
                Console.WriteLine("3 - Read last message");
                Console.WriteLine("4 - Delete last message");
                Console.WriteLine("5 - Move last message to Trash");
                Console.WriteLine("6 - Search messages");
                Console.WriteLine("7 - Show unread messages");
                Console.WriteLine("8 - Sort messages by date");
                Console.WriteLine("0 - Exit");

                Console.Write("Choose: ");
                string choice = Console.ReadLine();

                IMailFolder inbox = client.Inbox;

                switch (choice)
                {
                    case "1":
                        ShowFolders(client);
                        break;

                    case "2":
                        ShowMessages(inbox);
                        break;

                    case "3":
                        ReadLastMessage(inbox);
                        break;

                    case "4":
                        DeleteLastMessage(inbox);
                        break;

                    case "5":
                        MoveToTrash(client, inbox);
                        break;

                    case "6":
                        SearchMessages(inbox);
                        break;

                    case "7":
                        ShowUnread(inbox);
                        break;

                    case "8":
                        SortByDate(inbox);
                        break;

                    case "0":
                        running = false;
                        break;

                    default:
                        Console.WriteLine("Wrong choice");
                        break;
                }
            }

            client.Disconnect(true);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
        finally
        {
            client.Dispose();
        }
    }

    static void ShowFolders(ImapClient client)
    {
        Console.WriteLine("\nFolders:");
        var folders = client.GetFolders(client.PersonalNamespaces[0]);

        foreach (var folder in folders)
        {
            Console.WriteLine("- " + folder.Name);
        }
    }

    static void ShowMessages(IMailFolder inbox)
    {
        inbox.Open(FolderAccess.ReadOnly);
        var uids = inbox.Search(SearchQuery.All);

        Console.WriteLine("\nMessages:");

        List<UniqueId> list = uids.ToList();
        int count = list.Count;

        for (int i = Math.Max(0, count - 10); i < count; i++)
        {
            var m = inbox.GetMessage(list[i]);
            Console.WriteLine(m.Date + " | " + m.Subject);
        }
    }

    static void ReadLastMessage(IMailFolder inbox)
    {
        inbox.Open(FolderAccess.ReadOnly);
        var uids = inbox.Search(SearchQuery.All);

        if (uids.Count == 0) return;

        var msg = inbox.GetMessage(uids[uids.Count - 1]);

        Console.WriteLine("\nMessage:");
        Console.WriteLine("FROM: " + msg.From);
        Console.WriteLine("SUBJECT: " + msg.Subject);
        Console.WriteLine("TEXT:\n" + msg.TextBody);
    }

    static void DeleteLastMessage(IMailFolder inbox)
    {
        inbox.Open(FolderAccess.ReadWrite);
        var uids = inbox.Search(SearchQuery.All);

        if (uids.Count == 0) return;

        var last = uids[uids.Count - 1];

        inbox.AddFlags(last, MessageFlags.Deleted, true);
        inbox.Expunge();

        Console.WriteLine("Message deleted");
    }

    static void MoveToTrash(ImapClient client, IMailFolder inbox)
    {
        inbox.Open(FolderAccess.ReadWrite);
        var uids = inbox.Search(SearchQuery.All);

        if (uids.Count == 0) return;

        var last = uids[uids.Count - 1];

        var trash = client.GetFolder(SpecialFolder.Trash);

        inbox.MoveTo(last, trash);

        Console.WriteLine("Moved to Trash");
    }

    static void SearchMessages(IMailFolder inbox)
    {
        Console.Write("Enter text: ");
        string text = Console.ReadLine();

        inbox.Open(FolderAccess.ReadOnly);

        var results = inbox.Search(SearchQuery.BodyContains(text));

        Console.WriteLine("\nResults:");

        foreach (var uid in results)
        {
            var m = inbox.GetMessage(uid);
            Console.WriteLine(m.Date + " | " + m.Subject);
        }
    }

    static void ShowUnread(IMailFolder inbox)
    {
        inbox.Open(FolderAccess.ReadOnly);

        var unread = inbox.Search(SearchQuery.NotSeen);

        Console.WriteLine("\nUnread:");

        foreach (var uid in unread)
        {
            var m = inbox.GetMessage(uid);
            Console.WriteLine(m.Date + " | " + m.Subject);
        }
    }

    static void SortByDate(IMailFolder inbox)
    {
        inbox.Open(FolderAccess.ReadOnly);

        var list = inbox.Search(SearchQuery.All)
            .Select(uid => inbox.GetMessage(uid))
            .OrderBy(m => m.Date)
            .ToList();

        Console.WriteLine("\nSorted:");

        int count = list.Count;

        for (int i = Math.Max(0, count - 10); i < count; i++)
        {
            Console.WriteLine(list[i].Date + " | " + list[i].Subject);
        }
    }
}