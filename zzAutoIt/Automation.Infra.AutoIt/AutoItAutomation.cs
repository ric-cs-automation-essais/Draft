using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.Json;
using System.Threading;

using AutoIt;

using Automation.Infra.Interfaces;


namespace Automation.Infra.AutoIt
{
    public class AutoItAutomation : IAutomation
    {
        private readonly ScreensSet screensSet;

        public AutoItAutomation()
        {

            screensSet = new ScreensSet();
            var screen1 = new Screen { Id = 1, Width = 1920, Height = 1080 }; //Portable jb
            var screen2 = new Screen { Id = 2, Width = 1280, Height = 1024 }; //Old screen jb
            var screen3 = new Screen { Id = 3, Width = 1920, Height = 1200 }; //Main screen jb
            screensSet.AddScreenToTheRight(screen1);
            screensSet.AddScreenToTheRight(screen2);
            screensSet.AddScreenToTheRight(screen3, true);

            /*
            Thread.Sleep(1000);
            AutoItX.Send("^!c"); //CTRL+ALT+C
            Thread.Sleep(5000);
            AutoItX.Send("123{+}2=");
            Thread.Sleep(2000);
            AutoItX.Send("!{F4}"); //ALT+F4
            Thread.Sleep(2000);
            */


            var pos = AutoItX.MouseGetPos();
            Coords coordsInMainScreen = null;
            Coords currentCoordsInMainScreen;
            while (pos.X < 4000)
            {
                pos = AutoItX.MouseGetPos();
                currentCoordsInMainScreen = new Coords { X = pos.X, Y = pos.Y };
                if (currentCoordsInMainScreen != coordsInMainScreen)
                {
                    coordsInMainScreen = currentCoordsInMainScreen;

                    ShowScreen3CoordsToScreensCoords(coordsInMainScreen);
                    Console.WriteLine("\n\n");
                    ShowScreen2CoordsToMainScreenCoords(screensSet.GetMainScreenCoordsToScreenCoords(coordsInMainScreen, 2));
                    Console.WriteLine("\n\n");
                    ShowScreen1CoordsToMainScreenCoords(screensSet.GetMainScreenCoordsToScreenCoords(coordsInMainScreen, 1));
                    Console.WriteLine("\n========================================================\n\n");
                }

            }
            return;

            //AutoItX.MouseClickDrag("LEFT", 150, 629, 130, 828); //Drag and Drop
            //return;

            /*
            new MouseMoveOperations(new List<IMouseSetPositionOperation>
            {
                new AutoItMouseSetPositionOperation(10, 20, 5),
                new AutoItMouseSetPositionOperation(368, 1184, 50),
                //new AutoItMouseSetPositionOperation(200, 300, 100),
                //new AutoItMouseSetPositionOperation(500, 400, 2000),
                //new AutoItMouseSetPositionOperation(800, 600, 1000),
            }).execute();
            //AutoItX.MouseClick();
            */
        }


        private void ShowScreen3CoordsToScreensCoords(Coords coordsInMainScreen)
        {
            var messages = new[]
            {
                $"Coords sur screen 1   : {GetScreenCoordsAsText3(coordsInMainScreen, 1)}",
                $"Coords sur screen 2   : {GetScreenCoordsAsText3(coordsInMainScreen, 2)}",
                $"Coords sur screen 3   : {GetScreenCoordsAsText3(coordsInMainScreen, 3)}",
                $"Coords sur mainScreen : {JsonSerializer.Serialize(coordsInMainScreen)}",
                ""
            };

            var message = string.Join("\n", messages);
            Console.WriteLine(message);
        }
        private string GetScreenCoordsAsText3(Coords coordsInMainScreen, uint screenId)
        {
            return JsonSerializer.Serialize(screensSet.GetMainScreenCoordsToScreenCoords(coordsInMainScreen, screenId));
        }

        private void ShowScreen2CoordsToMainScreenCoords(Coords coordsInScreen2)
        {
            var messages = new[]
            {
                $"Coords sur screen 2   : {JsonSerializer.Serialize(coordsInScreen2)}",
                $"Coords sur mainScreen : {GetScreenCoordsAsText2(coordsInScreen2, 2)}",
                ""
            };

            var message = string.Join("\n", messages);
            Console.WriteLine(message);
        }
        private string GetScreenCoordsAsText2(Coords coordsInScreen, uint screenId)
        {
            return JsonSerializer.Serialize(screensSet.GetScreenCoordsToMainScreenCoords(coordsInScreen, screenId));
        }

        private void ShowScreen1CoordsToMainScreenCoords(Coords coordsInScreen1)
        {
            var messages = new[]
            {
                $"Coords sur screen 1   : {JsonSerializer.Serialize(coordsInScreen1)}",
                $"Coords sur mainScreen : {GetScreenCoordsAsText1(coordsInScreen1, 1)}",
                ""
            };

            var message = string.Join("\n", messages);
            Console.WriteLine(message);
        }
        private string GetScreenCoordsAsText1(Coords coordsInScreen, uint screenId)
        {
            return JsonSerializer.Serialize(screensSet.GetScreenCoordsToMainScreenCoords(coordsInScreen, screenId));
        }

    }
}






namespace Automation
{
    public class Screen
    {
        public uint Id { get; init; }
        public uint Width { get; init; }
        public uint Height { get; init; }

    }

    public record Coords
    {
        public int X { get; set; }
        public int Y { get; set; }
    }

    public class ScreensSet
    {
        private readonly List<Screen> screens;
        private readonly Dictionary<uint, int> screensPosition; //<screen.Id, screen position>
        private uint? mainScreenId = null;

        public ScreensSet()
        {
            screens = new List<Screen>();
            screensPosition = new Dictionary<uint, int>();
        }

        public void AddScreenToTheRight(Screen screen, bool isMainScreen = false)
        {
            if (screensPosition.ContainsKey(screen.Id))
            {
                throw new Exception($"L'écran d'Id {screen.Id} fait déjà partie du jeu d'écrans.");
            }
            if (isMainScreen)
            {
                if (IsDefinedMainScreen())
                {
                    throw new Exception($"L'écran principal a déjà été défini (Id = {mainScreenId.Value}).");
                }
                else
                {
                    mainScreenId = screen.Id;
                }
            }
            screens.Add(screen);
            var screenPosition = screens.Count - 1;
            screensPosition.Add(screen.Id, screenPosition);
        }


        //Conversion de Coords par rapport à l'écran principal, en Coords par rapport à l'écran d'Id screenId.
        public Coords GetMainScreenCoordsToScreenCoords(Coords coordsInMainScreen, uint screenId)
        {
            var screen = GetScreen(screenId);
            return GetMainScreenCoordsToScreenCoords(coordsInMainScreen, screen);
        }
        //Conversion de Coords par rapport à l'écran principal, en Coords par rapport à l'écran screen.
        public Coords GetMainScreenCoordsToScreenCoords(Coords coordsInMainScreen, Screen screen)
        {
            return GetScreen1CoordsToScreen2Coords(coordsInMainScreen, GetMainScreen(), screen);
        }
        //Conversion de Coords par rapport à l'écran d'Id screenId, en Coords par rapport à l'écran principal.
        public Coords GetScreenCoordsToMainScreenCoords(Coords coordsInScreen, uint screenId)
        {
            return GetScreen1CoordsToScreen2Coords(coordsInScreen, screenId, mainScreenId.Value);
        }

        //Conversion de Coords par rapport à l'écran d'Id screen1Id, en Coords par rapport à l'écran d'Id screen2Id.
        public Coords GetScreen1CoordsToScreen2Coords(Coords coordsInScreen1, uint screen1Id, uint screen2Id)
        {
            var screen1 = GetScreen(screen1Id);
            var screen2 = GetScreen(screen2Id);
            return GetScreen1CoordsToScreen2Coords(coordsInScreen1, screen1, screen2);
        }
        //Conversion de Coords par rapport à l'écran screen1, en Coords par rapport à l'écran screen2.
        public Coords GetScreen1CoordsToScreen2Coords(Coords coordsInScreen1, Screen screen1, Screen screen2)
        {
            Coords retour;

            var screen1Id = screen1.Id;
            var screen2Id = screen2.Id;

            if (!IsDefinedScreen(screen1Id))
            {
                throw new Exception($"Aucun écran d'Id '{screen1Id}' ne fait partie du jeu d'écrans.");
            }

            if (!IsDefinedScreen(screen2Id))
            {
                throw new Exception($"Aucun écran d'Id '{screen2Id}' ne fait partie du jeu d'écrans.");
            }

            retour = new Coords();

            var screen1Position = GetScreenPosition(screen1Id);
            var screen2Position = GetScreenPosition(screen2Id);

            if (IsScreen2ToTheLeftOfScreen1(screen1Id, screen2Id))
            {
                var fromScreenPosition = screen2Position;
                var toScreenPosition = screen1Position - 1;

                retour.X = coordsInScreen1.X + Convert.ToInt32(GetScreensWidthSum(fromScreenPosition, toScreenPosition));
            }
            else
            {
                var fromScreenPosition = screen1Position;
                var toScreenPosition = screen2Position - 1;

                retour.X = coordsInScreen1.X - Convert.ToInt32(GetScreensWidthSum(fromScreenPosition, toScreenPosition));
            }

            retour.Y = Math.Min(Convert.ToInt32(screen2.Height), coordsInScreen1.Y);

            return retour;
        }

        private uint GetScreensWidthSum(int fromScreenPosition, int toScreenPosition)
        {
            if (fromScreenPosition > toScreenPosition)
            {
                new ArgumentException($"fromScreenPosition ('{fromScreenPosition}') doit être inférieur ou égale à toScreenPosition ('{toScreenPosition}').");
            }

            var retour = screens.Skip(fromScreenPosition).Take(toScreenPosition - fromScreenPosition + 1).Sum(screen => screen.Width);
            return Convert.ToUInt32(retour);
        }

        private bool IsScreenToTheLeftOfTheMainScreen(uint screenId)
        {
            var retour = IsScreen2ToTheLeftOfScreen1(mainScreenId.Value, screenId);
            return retour;
        }

        private bool IsScreen2ToTheLeftOfScreen1(uint screen1Id, uint screen2Id)
        {
            var screen1Position = GetScreenPosition(screen1Id);
            var screen2Position = GetScreenPosition(screen2Id);

            var retour = (screen2Position < screen1Position);
            return retour;
        }

        private bool IsMainScreen(uint screenId)
        {
            return screenId == mainScreenId.Value;
        }

        private Screen GetMainScreen()
        {
            var retour = GetScreen(mainScreenId.Value);
            return retour;
        }

        public Screen GetScreen(uint screenId)
        {
            var retour = screens.Single(screen => screen.Id == screenId);
            return retour;
        }

        private int GetMainScreenPosition()
        {
            int retour = GetScreenPosition(mainScreenId.Value);
            return retour;
        }

        private int GetScreenPosition(uint screenId)
        {
            int retour;
            screensPosition.TryGetValue(screenId, out retour);
            return retour;
        }

        private bool IsDefinedScreen(uint screenId)
        {
            var retour = screensPosition.ContainsKey(screenId);
            return retour;
        }

        private bool IsDefinedMainScreen()
        {
            return mainScreenId is not null;
        }
   }

    public interface IOperation
    {
        bool execute();
    }

    public interface IMouseSetPositionOperation : IOperation
    {
    }
    public interface IMouseMoveOperation : IOperation
    {
    }

    public interface IAutomate
    {
        bool executeOperation(IOperation operation);
    }

    //public class AutoItAutomateOperationsFactory
    //{
    //    public AutoItAutomateOperationsFactory()
    //    {

    //    }

    //    IMouseSetPositionOperation CreateMouseSetPositionOperation(int x, int y)
    //    {
    //        var retour = new AutoItMouseSetPositionOperation(x, y);
    //        return retour;
    //    }

    //}

    public abstract class AMouseSetPositionOperation
    {
        private readonly int delayAfterPositionning;

        protected readonly Point coord;

        protected abstract bool _execute();

        protected AMouseSetPositionOperation(Point coord, int delayAfterPositionning = 0)
        {
            this.coord = coord;
            this.delayAfterPositionning = delayAfterPositionning;
        }
        protected AMouseSetPositionOperation(int x, int y, int delayAfterPositionning = 0) : this(new Point { X = x, Y = y }, delayAfterPositionning)
        {
        }

        public bool execute()
        {
            var retour = _execute();
            DelayAfterPositionning();
            return retour;
        }

        private void DelayAfterPositionning()
        {
            Thread.Sleep(delayAfterPositionning);
        }
    }

    public class AutoItMouseSetPositionOperation : AMouseSetPositionOperation, IMouseSetPositionOperation
    {
        public AutoItMouseSetPositionOperation(Point coord, int delayAfterPositionning = 0) : base(coord, delayAfterPositionning)
        {
        }
        public AutoItMouseSetPositionOperation(int x, int y, int delayAfterPositionning = 0) : base(x, y, delayAfterPositionning)
        {
        }

        protected override bool _execute()
        {
            AutoItX.MouseMove(coord.X, coord.Y);

            //var pos = AutoItX.MouseGetPos(); Console.WriteLine($"X={coord.X} ; Y={coord.Y}  / {coord.X==pos.X};{coord.Y==pos.Y}");
            return true;
        }
    }


    public class Operations<TOperation> : IOperation
        where TOperation : IOperation
    {
        private readonly IList<TOperation> operations;

        public Operations(IList<TOperation> operations)
        {
            this.operations = operations;
        }

        public bool execute()
        {
            bool retour = true;

            foreach (var operation in operations)
            {
                retour = operation.execute();
                if (!retour)
                {
                    break;
                }
            }

            return retour;
        }
    }

    public class MouseMoveOperations : Operations<IMouseSetPositionOperation>
    {
        public MouseMoveOperations(IList<IMouseSetPositionOperation> mouseSetPositionOperations) : base(mouseSetPositionOperations)
        {
        }
    }
}
