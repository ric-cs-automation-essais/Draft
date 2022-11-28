using System;
using System.Drawing;
using Automation.Infra.Interfaces;

using AutoIt;
using System.Threading;
using System.Collections.Generic;

namespace Automation.Infra.AutoIt
{
    public class AutoItAutomation : IAutomation
    {
        public AutoItAutomation()
        {
            new MouseMoveOperations(new List<IMouseSetPositionOperation>
            {
                new AutoItMouseSetPositionOperation(10, 20, 5),
                new AutoItMouseSetPositionOperation(100, 200, 50),
                new AutoItMouseSetPositionOperation(200, 300, 100),
                new AutoItMouseSetPositionOperation(500, 400, 2000),
                new AutoItMouseSetPositionOperation(800, 600, 1000),
            }).execute();
        }
    }
}





namespace Automation
{
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
