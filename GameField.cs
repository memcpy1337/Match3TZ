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
        private Cell[,] grid = new Cell[10, 10];

        private List<Destroyer> destroyers = new List<Destroyer>();
        private List<Bomb> bombs = new List<Bomb>();

        private HashSet<Cell> destroyedCellsThisMoveHash = new HashSet<Cell>();

        private HashSet<Cell> matchesVerticalHash = new HashSet<Cell>();
        private HashSet<Cell> matchesHorizontalHash = new HashSet<Cell>();

        private ClickGameField clickField = new ClickGameField();

        private bool isSwap = false, isMoving = false;
        private int score = 0;

        public GameField(int sizeCell, Vector2i fieldOffset, Window gameWindow, Action<int> scoreUpdate, Action<float> timerUpdate)
        {
            _size = sizeCell;
            _offset = fieldOffset;
            this.gameWindow = gameWindow;
            this.scoreUpdate = scoreUpdate;
            this.timerUpdate = timerUpdate;
            ControlManager.MouseClick += MouseClick;

            SpritePool.CreatePool();

            for (int x = 0; x < 10; x++)
            {
                for (int y = 0; y < 10; y++)
                {
                    grid[x, y] = new Cell();
                }
            }

            for (int x = 1; x <= 8; x++)
            {
                for (int y = 1; y <= 8; y++)
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

            if (isSwap && isMoving)
            {
                return;
            }

            clickField.Pos = e.MousePosition - _offset;

            if (clickField.IsClick == false)
            {
                clickField.x0 = (clickField.Pos.X + 16) / _size + 1;
                clickField.y0 = (clickField.Pos.Y + 16) / _size + 1;

                if (clickField.InBounds(clickField.x0, clickField.y0) == false)
                {
                    return;
                }

                var cell = grid[clickField.y0, clickField.x0];

                if (cell.Equals(GetSelectedCell()) == false)
                {
                    cell.IsSelected = true;
                    clickField.IsClick = true;
                }
            }

            else if (clickField.IsClick == true)
            {
                clickField.x = (clickField.Pos.X + 16) / _size + 1;
                clickField.y = (clickField.Pos.Y + 16) / _size + 1;


                var cell = grid[clickField.y, clickField.x];

                if (clickField.InBounds(clickField.x, clickField.y) == false && cell.Equals(GetSelectedCell()))
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
                for (int x = 1; x <= 8; x++)
                {
                    for (int y = 1; y <= 8; y++)
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
            for (int x = 1; x <= 8; x++)
            {
                for (int y = 1; y <= 8; y++)
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
            for (int x = 1; x <= 8; x++)
            {
                for (int y = 1; y <= 8; y++)
                {
                    var cell = grid[x, y];

                    if (cell.HandleAnim())
                        isMoving = true;

                }
            }
        }
        private int HandleMatchRecursive(int x, int y, CellTypes targetType, HashSet<Cell> matchedCells, int dx, int dy)
        {
            if (x < 1 || x > 8 || y < 1 || y > 8 || grid[x, y].Type != targetType || matchedCells.Contains(grid[x, y]) || grid[x, y].Hit > 0)
            {
                return 0;
            }

            matchedCells.Add(grid[x, y]);

            int count = 1 + HandleMatchRecursive(x + dx, y + dy, targetType, matchedCells, dx, dy);

            return count;
        }

        private void HandleMatch()
        {
            for (int x = 1; x <= 8; x++)
            {
                for (int y = 1; y <= 8; y++)
                {
                    CellTypes targetType = grid[x, y].Type;

                    matchesHorizontalHash.Clear();
                    matchesVerticalHash.Clear();

                    int countVertical = CountMatches(x, y, targetType, matchesVerticalHash, 0, 1)
                                      + CountMatches(x, y - 1, targetType, matchesVerticalHash, 0, -1);

                    int countHorizontal = CountMatches(x, y, targetType, matchesHorizontalHash, 1, 0)
                                        + CountMatches(x - 1, y, targetType, matchesHorizontalHash, -1, 0);

                    if (countVertical >= 3)
                    {
                        ProcessMatches(matchesVerticalHash, countVertical, true);
                    }
                    else if (countHorizontal >= 3)
                    {
                        ProcessMatches(matchesHorizontalHash, countHorizontal, false);
                    }
                }
            }
        }

        private int CountMatches(int startX, int startY, CellTypes targetType, HashSet<Cell> matchesHash, int offsetX, int offsetY)
        {
            int count = HandleMatchRecursive(startX, startY, targetType, matchesHash, offsetX, offsetY);
            count += HandleMatchRecursive(startX - offsetX, startY - offsetY, targetType, matchesHash, -offsetX, -offsetY);
            return count;
        }

        private void ProcessMatches(HashSet<Cell> matchesHash, int matchCount, bool isVertical)
        {
            if (matchCount == 3)
            {
                foreach (var cell in matchesHash)
                {
                    ProcessCell(cell);
                }
            }
            else if (matchCount == 4)
            {
                var cellToBonus = matchesHash.OrderByDescending(x => x.TimeLastMoved).First();
                cellToBonus.Bonus = isVertical ? Bonus.LineVertical : Bonus.LineHorizontal;
                matchesHash.Remove(cellToBonus);

                foreach (var cell in matchesHash)
                {
                    ProcessCell(cell);
                }
            }
            else if (matchCount > 4)
            {
                var cellToBonus = matchesHash.OrderByDescending(x => x.TimeLastMoved).First();
                cellToBonus.Bonus = Bonus.Bomb;
                matchesHash.Remove(cellToBonus);

                foreach (var cell in matchesHash)
                {
                    ProcessCell(cell);
                }
            }
        }

        private void ProcessCell(Cell cell)
        {
            grid[cell.Row, cell.Col].Hit++;
            destroyedCellsThisMoveHash.Add(grid[cell.Row, cell.Col]);
            HandleBonus(cell);
        }

        private void CreateDestroyer(int Row, int Col, Bonus bonus)
        {
            if (bonus == Bonus.LineVertical)
            {
                var destroyer1 = new Destroyer(Row, Col, Col, 1, DestroyerTarget, DestroyDestroyer);
                var destroyer2 = new Destroyer(Row, Col, Col, 8, DestroyerTarget, DestroyDestroyer);
                destroyers.Add(destroyer1);
                destroyers.Add(destroyer2);
            }
            else if (bonus == Bonus.LineHorizontal)
            {
                var destroyer1 = new Destroyer(Row, Col, 1, Row, DestroyerTarget, DestroyDestroyer);
                var destroyer2 = new Destroyer(Row, Col, 8, Row, DestroyerTarget, DestroyDestroyer);
                destroyers.Add(destroyer1);
                destroyers.Add(destroyer2);
            }

        }

        private void CreateBomb(int Row, int Col)
        {
            var bomb = new Bomb(Row, Col, DestroyBomb);
            bombs.Add(bomb);
        }

        private void DestroyBomb(Bomb bomb)
        {
            int bombRow = bomb.Row;
            int bombCol = bomb.Col;

            for (int row = Math.Max(1, bombRow - 1); row <= Math.Min(8, bombRow + 1); row++)
            {
                for (int col = Math.Max(1, bombCol - 1); col <= Math.Min(8, bombCol + 1); col++)
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
            for (int x = 1; x <= 8; x++)
            {
                for (int y = 1; y <= 8; y++)
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
                for (int x = 8; x > 0; x--)
                {
                    for (int y = 1; y <= 8; y++)
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

                for (int y = 1; y <= 8; y++)
                {
                    int n = 0;
                    for (int x = 8; x > 0; x--)
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
