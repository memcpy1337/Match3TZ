using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace Match3TZ
{

    internal class GameField : Transformable, Drawable
    {
        private Vector2i _offset;
        private int _size;
        private Window gameWindow;
        private Random random = new Random();
        private Action<int> scoreUpdate;
        private Action<float> timerUpdate;

        private Clock clock = new Clock();
        private Cell[,] grid;

        private List<Destroyer> destroyers = new List<Destroyer>();
        private List<Bomb> bombs = new List<Bomb>();

        private HashSet<Cell> destroyedCellsThisMoveHash = new HashSet<Cell>();

        private HashSet<Cell> matchesVerticalHash = new HashSet<Cell>();
        private HashSet<Cell> matchesHorizontalHash = new HashSet<Cell>();

        private ClickGameField clickField = new ClickGameField();

        private bool isSwap = false, isMoving = false;
        private int score = 0;

        public GameField(int sizeCell, int Rows, int Cols, Vector2i fieldOffset, RenderWindow gameWindow, Action<int> scoreUpdate, Action<float> timerUpdate)
        {
            _size = sizeCell;
            _offset = fieldOffset;

            grid = new Cell[Rows + 3, Cols + 3];

            this.gameWindow = gameWindow;
            this.scoreUpdate = scoreUpdate;
            this.timerUpdate = timerUpdate;

            ControlManager.MouseClick += MouseClick;

            SpritePool.CreatePool();

            for (int x = 0; x <= grid.GetUpperBound(0); x++)
            {
                for (int y = 0; y <= grid.GetUpperBound(1); y++)
                {
                    grid[x, y] = new Cell();
                }
            }

            for (int x = 1; x <= grid.GetUpperBound(0) - 2; x++)
            {
                for (int y = 1; y <= grid.GetUpperBound(1) - 2; y++)
                {
                    grid[x, y].ChangeType((CellTypes)random.Next(0, 5));
                    grid[x, y].SetPosition(x, y);
                }
            }

        }

        public void Destroy()
        {
            SpritePool.DestroyPool();
            ControlManager.MouseClick -= MouseClick;
        }

        private void MouseClick(object? sender, MouseClickEventArgs e)
        {

            if (isSwap || isMoving)
            {
                return;
            }

            clickField.Pos = e.MousePosition - _offset;

            int x = (clickField.Pos.X + 16) / _size + 1;
            int y = (clickField.Pos.Y + 16) / _size + 1;

            if (clickField.IsClick == false)
            {
                clickField.x0 = (clickField.Pos.X + 16) / _size + 1;
                clickField.y0 = (clickField.Pos.Y + 16) / _size + 1;

                if (InBounds(clickField.y0, clickField.x0) == false)
                {
                    return;
                }

                var cell = grid[clickField.y0, clickField.x0];

                if (cell.Equals(GetSelectedCell()) == false)
                {
                    Console.WriteLine($"Clicked on ROW:{cell.Row} COL:{cell.Col} COLOR:{cell.Type}");
                    cell.IsSelected = true;
                    clickField.IsClick = true;
                }
            }

            else if (clickField.IsClick == true)
            {
                clickField.x = (clickField.Pos.X + 16) / _size + 1;
                clickField.y = (clickField.Pos.Y + 16) / _size + 1;

                if (InBounds(clickField.y, clickField.x) == false)
                {
                    return;
                }

                var cell = grid[clickField.y, clickField.x];

                if (cell.Equals(GetSelectedCell()))
                {
                    return;
                }

                clickField.IsClick = false;
                var selectedCell = GetSelectedCell();
                selectedCell.IsSelected = false;

                if (clickField.IsExist())
                {
                    destroyedCellsThisMoveHash.Clear();
                    ChangeCells(grid[clickField.y0, clickField.x0], grid[clickField.y, clickField.x]);
                    isSwap = true;

                }
            }
        }

        public void Update()
        {
            HandleClickCheat();
            HandleMatch();
            HandleAnim();

            if (!isMoving)
            {
                for (int x = 1; x <= grid.GetUpperBound(0) - 2; x++)
                {
                    for (int y = 1; y <= grid.GetUpperBound(1) - 2; y++)
                    {
                        if (grid[x, y].Hit != 0)
                        {
                            if (grid[x, y].Alpha > 10)
                            {
                                grid[x, y].Alpha -= 10;
                                isMoving = true;
                            }
                        }
                    }
                }
            }


            if (isSwap && !isMoving)
            {
                if (destroyedCellsThisMoveHash.Count == 0)
                {
                    ChangeCells(grid[clickField.y0, clickField.x0], grid[clickField.y, clickField.x]);
                    isSwap = false;
                }
            }


            HandleUpdateGrid();

            foreach (var destroyer in destroyers.ToList())
            {
                destroyer.Update();
            }

            foreach (var bomb in bombs.ToList())
            {
                bomb.Update();
            }

            timerUpdate.Invoke(clock.ElapsedTime.AsSeconds());

        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            for (int x = 1; x <= grid.GetUpperBound(0) - 2; x++)
            {
                for (int y = 1; y <= grid.GetUpperBound(1) - 2; y++)
                {
                    var cell = grid[x, y];

                    target.Draw(cell);
                }
            }

            foreach (var destroy in destroyers)
            {
                target.Draw(destroy);
            }
        }

        private void ChangeCells(Cell cell1, Cell cell2)
        {
            (cell1.Col, cell2.Col) = (cell2.Col, cell1.Col);
            (cell1.Row, cell2.Row) = (cell2.Row, cell1.Row);

            grid[cell1.Row, cell1.Col] = cell1;
            grid[cell2.Row, cell2.Col] = cell2;

            cell1.SetTimeMoved(clock);
            cell2.SetTimeMoved(clock);
        }

        private void HandleClickCheat()
        {
            bool isButtonPressed = Mouse.IsButtonPressed(Mouse.Button.Right);
            Vector2i mousePosition = Mouse.GetPosition(gameWindow);

            if (mousePosition.X >= 0 && mousePosition.X < gameWindow.Size.X &&
                mousePosition.Y >= 0 && mousePosition.Y < gameWindow.Size.Y)
            {
                if (isButtonPressed)
                {
                    if (!isSwap && !isMoving)
                    {
                        clickField.Pos = Mouse.GetPosition(gameWindow) - _offset;
                        clickField.x0 = (clickField.Pos.X + 16) / _size + 1;
                        clickField.y0 = (clickField.Pos.Y + 16) / _size + 1;

                        var cell = grid[clickField.y0, clickField.x0];
                        cell.ChangeType(CellTypes.Green);
                    }

                }
            }
        }

        public bool InBounds(int x, int y)
        {
            return x >= 1 && x <= grid.GetUpperBound(0) - 2 && y >= 1 && y <= grid.GetUpperBound(1) - 2;
        }

        private void HandleBonus(Cell cell)
        {
            switch (cell.Bonus)
            {
                case Bonus.Empty:
                    break;
                case Bonus.LineVertical:
                    cell.Bonus = Bonus.Empty;
                    CreateDestroyer(cell.Row, cell.Col, Bonus.LineVertical);
                    break;
                case Bonus.LineHorizontal:
                    cell.Bonus = Bonus.Empty;
                    CreateDestroyer(cell.Row, cell.Col, Bonus.LineHorizontal);
                    break;
                case Bonus.Bomb:
                    cell.Bonus = Bonus.Empty;
                    CreateBomb(cell.Row, cell.Col);
                    break;
            }
        }

        private void HandleAnim()
        {
            isMoving = false;
            for (int x = 1; x <= grid.GetUpperBound(0) - 2; x++)
            {
                for (int y = 1; y <= grid.GetUpperBound(1) - 2; y++)
                {
                    var cell = grid[x, y];

                    if (cell.HandleAnim())
                        isMoving = true;

                }
            }
        }

        Queue<Cell> queue = new Queue<Cell>();
        HashSet<Cell> region = new HashSet<Cell>();
        List<Cell> cellToDestroyR = new List<Cell>();
        List<Cell> cellToDestroyC = new List<Cell>();
        private void HandleMatch()
        {
            if (bombs.Any() || destroyers.Any())
                return;

            bool[,] visited = new bool[grid.GetLength(0) - 2, grid.GetLength(1) - 2];

            for (int x = 1; x <= grid.GetUpperBound(0) - 2; x++)
            {
                for (int y = 1; y <= grid.GetUpperBound(1) - 2; y++)
                {
                    if (visited[x, y]) continue;  // Пропуск уже обработанных клеток

                    CellTypes targetType = grid[x, y].Type;

                    // Очередь для flood fill
                    queue.Clear();
                    // Посещенные клетки за этот обход
                    region.Clear();

                    queue.Enqueue(grid[x, y]);

                    while (queue.Count > 0)
                    {
                        var cell = queue.Dequeue();

                        if (visited[cell.Row, cell.Col]) continue;

                        visited[cell.Row, cell.Col] = true;
                        region.Add(cell);

                        // Проверка соседних клеток
                        if (cell.Row - 1 >= 0 && !visited[cell.Row - 1, cell.Col] && grid[cell.Row - 1, cell.Col].Type == targetType)
                        {
                            queue.Enqueue(grid[cell.Row - 1, cell.Col]);
                        }
                        if (cell.Row + 1 < grid.GetLength(0) - 2 && !visited[cell.Row + 1, cell.Col] && grid[cell.Row + 1, cell.Col].Type == targetType)
                        {
                            queue.Enqueue(grid[cell.Row + 1, cell.Col]);
                        }
                        if (cell.Col - 1 >= 0 && !visited[cell.Row, cell.Col - 1] && grid[cell.Row, cell.Col - 1].Type == targetType)
                        {
                            queue.Enqueue(grid[cell.Row, cell.Col - 1]);
                        }
                        if (cell.Col + 1 < grid.GetLength(1) - 2 && !visited[cell.Row, cell.Col + 1] && grid[cell.Row, cell.Col + 1].Type == targetType)
                        {
                            queue.Enqueue(grid[cell.Row, cell.Col + 1]);
                        }
                    }

                    // Сегментация клеток в найденных чанках
                    if (region.Count >= 3)
                    {
                        var arr = region.ToList();
                        cellToDestroyR.Clear();
                        cellToDestroyC.Clear();

                        for (int row = arr.Min(c => c.Row); row <= arr.Max(c => c.Row); row++)
                        {
                            var rowMatches = arr.Where(c => c.Row == row).OrderBy(c => c.Col).ToList();
                            if (rowMatches.Count >= 3)
                            {
                                cellToDestroyR.AddRange(rowMatches);
                            }
                        }

                        for (int col = arr.Min(c => c.Col); col <= arr.Max(c => c.Col); col++)
                        {
                            var colMatches = arr.Where(c => c.Col == col).OrderBy(c => c.Row).ToList();
                            if (colMatches.Count >= 3)
                            {
                                cellToDestroyC.AddRange(colMatches);
                            }
                        }

                        // Удаление дублей
                        cellToDestroyR = cellToDestroyR.Distinct().ToList();
                        cellToDestroyC = cellToDestroyC.Distinct().ToList();

                        // Обработка матчей
                        if (cellToDestroyC.Count >= 3)
                        {
                            ProcessMatches(cellToDestroyC, false);
                        }

                        if (cellToDestroyR.Count >= 3)
                        {
                            ProcessMatches(cellToDestroyR, true);
                        }
                    }
                }
            }
        }


        private void ProcessMatches(List<Cell> matchesHash, bool isVertical)
        {
            int matchCount = matchesHash.Count;

            if (matchCount >= 3)
            {
                var cellsToProcess = matchesHash.OrderByDescending(x => x.TimeLastMoved).ToList();

                if (matchCount == 4)
                {
                    var cellToBonus = cellsToProcess.First();
                    cellToBonus.Bonus = isVertical ? Bonus.LineVertical : Bonus.LineHorizontal;
                    cellsToProcess.Remove(cellToBonus);
                }
                else if (matchCount > 4)
                {
                    var cellToBonus = cellsToProcess.First();
                    cellToBonus.Bonus = Bonus.Bomb;
                    cellsToProcess.Remove(cellToBonus);
                }

                foreach (var cell in cellsToProcess)
                {
                    ProcessCell(cell);
                }
            }
        }

        private void ProcessCell(Cell cell)
        {
            if (cell == null)
                return;

            grid[cell.Row, cell.Col].Hit++;
            destroyedCellsThisMoveHash.Add(grid[cell.Row, cell.Col]);
            HandleBonus(cell);
        }

        private void CreateDestroyer(int Row, int Col, Bonus bonus)
        {
            Console.WriteLine($"Создан Destroyer ROW:{Row} COL:{Col} TYPE:{bonus}");

            if (bonus == Bonus.LineVertical)
            {
                var destroyer1 = new Destroyer(Row, Col, Col, 1, DestroyerTarget, DestroyDestroyer);
                var destroyer2 = new Destroyer(Row, Col, Col, grid.GetUpperBound(0) - 2, DestroyerTarget, DestroyDestroyer);
                destroyers.Add(destroyer1);
                destroyers.Add(destroyer2);
            }
            else if (bonus == Bonus.LineHorizontal)
            {
                var destroyer1 = new Destroyer(Row, Col, 1, Row, DestroyerTarget, DestroyDestroyer);
                var destroyer2 = new Destroyer(Row, Col, grid.GetUpperBound(1) - 2, Row, DestroyerTarget, DestroyDestroyer);
                destroyers.Add(destroyer1);
                destroyers.Add(destroyer2);
            }

        }

        private void CreateBomb(int Row, int Col)
        {
            Console.WriteLine($"Создан Bomb ROW:{Row} COL:{Col}");

            var bomb = new Bomb(Row, Col, DestroyBomb);
            bombs.Add(bomb);
        }

        private void DestroyBomb(Bomb bomb)
        {
            int bombRow = bomb.Row;
            int bombCol = bomb.Col;

            for (int row = Math.Max(1, bombRow - 1); row <= Math.Min(grid.GetUpperBound(0) - 2, bombRow + 1); row++)
            {
                for (int col = Math.Max(1, bombCol - 1); col <= Math.Min(grid.GetUpperBound(1) - 2, bombCol + 1); col++)
                {
                    grid[row, col].Hit++;
                    destroyedCellsThisMoveHash.Add(grid[row, col]);
                    HandleBonus(grid[row, col]);
                }
            }
            bombs.Remove(bomb);
            bomb.Dispose();
        }

        private Cell GetSelectedCell()
        {
            for (int x = 1; x <= grid.GetUpperBound(0) - 2; x++)
            {
                for (int y = 1; y <= grid.GetUpperBound(1) - 2; y++)
                {
                    if (grid[x ,y].IsSelected == true)
                    {
                        return grid[x, y];
                    }

                }
            }
            return null;
        }

        private void DestroyerTarget((int x, int y) cell)
        {
            grid[cell.y, cell.x].Hit++;
            destroyedCellsThisMoveHash.Add(grid[cell.y, cell.x]);
            HandleBonus(grid[cell.y, cell.x]);
        }

        private void DestroyDestroyer(Destroyer destr)
        {
            destroyers.Remove(destr);
            destr.Dispose();
        }

        private void HandleUpdateGrid()
        {
            if (!isMoving && destroyers.Count == 0 && bombs.Count == 0)
            {
                for (int x = grid.GetUpperBound(0) - 2; x > 0; x--)
                {
                    for (int y = 1; y <= grid.GetUpperBound(1) - 2; y++)
                    {
                        if (grid[x, y].Hit != 0)
                        {
                            for (int n = x; n > 0; n--)
                            {
                                if (grid[n, y].Hit == 0)
                                {
                                    ChangeCells(grid[n, y], grid[x, y]);
                                    break;
                                }
                            }
                        }
                    }
                }

                for (int y = 1; y <= grid.GetUpperBound(1) - 2; y++)
                {
                    int n = 0;
                    for (int x = grid.GetUpperBound(0) - 2; x > 0; x--)
                    {
                        if (grid[x, y].Hit != 0)
                        {
                            grid[x, y].ChangeType((CellTypes)random.Next(0, 5));
                            grid[x, y].SetY(-_size * n++);
                            grid[x, y].Hit = 0;
                            grid[x, y].Alpha = 255;
                            score += destroyedCellsThisMoveHash.Count;
                            scoreUpdate?.Invoke(score);
                            destroyedCellsThisMoveHash.Clear();
                        }
                    }
                }
                isSwap = false;
            }
        }

    }
}
