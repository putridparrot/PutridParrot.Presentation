//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace PutridParrot.Presentation.Helpers
//{
//    // taken from https://www.codeproject.com/articles/456591/simple-undo-redo-library-for-csharp-net
//    public class StackFixedSize<T> : LinkedList<T>
//    {
//        private int _maxSize;

//        public StackFixedSize(int maxSize)
//        {
//            MaxSize = maxSize;
//        }

//        public int MaxSize
//        {
//            get { return _maxSize; }
//            set
//            {
//                _maxSize = value;
//                if (_maxSize <= 0)
//                {
//                    throw new InvalidOperationException("MaxSize must be positive, non-zero");
//                }

//                GuardSize();
//            }
//        }

//        public void Push(T instance)
//        {
//            AddFirst(instance);
//            GuardSize();
//        }

//        private void GuardSize()
//        {
//            while (Count > _maxSize)
//            {
//                RemoveLast();
//            }
//        }

//        public T Pop()
//        {
//            var instance = First.Value;
//            RemoveFirst();
//            return instance;
//        }

//        public T Peek()
//        {
//            return First != null ? First.Value : default(T);
//        }
//    }

//    public interface IUndoRedoCommand<T>
//    {
//        T Do(T value);
//        T Undo(T value);
//    }

//    // based heavily on https://www.cambiaresearch.com/articles/82/generic-undoredo-stack-in-csharp
//    public class UndoRedoStack<T>
//    {
//        private readonly Stack<IUndoRedoCommand<T>> _undoStack;
//        private readonly Stack<IUndoRedoCommand<T>> _redoStack;

//        public UndoRedoStack()
//        {
//            _undoStack = new Stack<IUndoRedoCommand<T>>();
//            _redoStack = new Stack<IUndoRedoCommand<T>>();
//        }

//        public int UndoCount => _undoStack?.Count ?? 0;
//        public int RedoCount => _redoStack?.Count ?? 0;

//        public void Clear()
//        {
//            _undoStack.Clear();
//            _redoStack.Clear();
//        }

//        public T Do(IUndoRedoCommand<T> command, T value)
//        {
//            var result = command.Do(value);
//            _undoStack.Push(command);
//            _redoStack.Clear();
//            return result;
//        }

//        public T Undo(T value)
//        {
//            if (_undoStack.Count > 0)
//            {
//                var command = _undoStack.Pop();
//                var result = command.Undo(value);
//                _redoStack.Push(command);
//                return result;
//            }
//            return value;
//        }

//        public T Redo(T value)
//        {
//            if (_redoStack.Count > 0)
//            {
//                var command = _redoStack.Pop();
//                var result = command.Do(value);
//                _undoStack.Push(command);
//                return result;
//            }
//            return value;
//        }
//    }
//}
