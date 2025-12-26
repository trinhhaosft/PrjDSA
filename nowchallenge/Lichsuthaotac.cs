using System;
using System.Collections.Generic;

namespace nowchallenge
{
    public class LichSuThaoTac
    {
        class Operation
        {
            public Action Do { get; }
            public Action Undo { get; }
            public Operation(Action doAction, Action undoAction) { Do = doAction; Undo = undoAction; }
        }

        private Stack<Operation> undoStack = new Stack<Operation>();
        private Stack<Operation> redoStack = new Stack<Operation>();
        private readonly object sync = new object();
        private const int MaxSteps = 50;

        // Thể hiện Singleton để dễ dàng truy cập từ các lớp khác
        private static readonly Lazy<LichSuThaoTac> lazy = new Lazy<LichSuThaoTac>(() => new LichSuThaoTac());
        public static LichSuThaoTac Instance => lazy.Value;

        private LichSuThaoTac() { }

        // Đăng ký thao tác và thực thi doAction ngay lập tức
        public void AddOperation(Action doAction, Action undoAction)
        {
            if (doAction == null) throw new ArgumentNullException(nameof(doAction));
            if (undoAction == null) throw new ArgumentNullException(nameof(undoAction));

            lock (sync)
            {
                // thực thi rồi đẩy vào stack
                doAction();
                undoStack.Push(new Operation(doAction, undoAction));
                redoStack.Clear();
                // cắt bớt nếu vượt quá giới hạn
                while (undoStack.Count > MaxSteps)
                {
                    var arr = undoStack.ToArray();
                    var list = new List<Operation>(arr);
                    list.RemoveAt(list.Count - 1); // xóa thao tác cũ nhất
                    list.Reverse();
                    undoStack = new Stack<Operation>(list);
                }
            }
        }

        public void Undo()
        {
            lock (sync)
            {
                if (undoStack.Count == 0) return;
                var op = undoStack.Pop();
                try
                {
                    op.Undo();
                    redoStack.Push(op);
                }
                catch (Exception) {/* ghi log nếu cần */ }
            }
        }

        public void Redo()
        {
            lock (sync)
            {
                if (redoStack.Count == 0) return;
                var op = redoStack.Pop();
                try
                {
                    op.Do();
                    undoStack.Push(op);
                }
                catch (Exception) { /* ghi log nếu cần */ }
            }
        }
        
    }
}
