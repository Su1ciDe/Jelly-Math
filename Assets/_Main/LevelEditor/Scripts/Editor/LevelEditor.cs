using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Fiber.Utilities;
using Fiber.LevelSystem;
using GamePlay;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UIElements;
using Grid = GridSystem.Grid;
using Object = UnityEngine.Object;

namespace LevelEditor
{
	public class LevelEditor : EditorWindow
	{
		private static LevelEditor window;

		#region Elements

		[SerializeField] private VisualTreeAsset treeAsset;

		private VisualElement MainVisualElement;
		private VisualElement MainTabsRowVisualElement;
		private VisualElement MainGridVisualElement, MainDeckVisualElement;

		// Deck
		private ListView listViewShapes;
		private VisualElement DeckTabsRowVisualElement;
		private Button btn_AddDeckTab;
		private VisualElement DeckVisualElement;

		private TextField txt_DeckX, txt_DeckY;
		private TextField txt_ShapeValue;
		private Button btn_DeckSetup;

		// Grid
		private ListView listViewHolders;

		private ColorField ColorPicker;

		private VisualElement TabsRowVisualElement;
		private Button btn_AddTab;

		private VisualElement GridVisualElement;

		// Options
		private TextField txt_LevelTime;
		private TextField txt_LevelNo;
		private Button btn_Save;

		#endregion

		private Color selectedColor = Color.black;
		private Dictionary<Color, GridNodeInfo> colorHolderPair = new Dictionary<Color, GridNodeInfo>();

		private int levelNo;

		private Directions direction;

		private const float CELL_SIZE = 50;
		private const float PIECE_SIZE = 1;

		#region Paths

		private const string LEVELS_PATH = "Assets/_Main/Prefabs/Levels/";
		private static readonly string LEVEL_BASE_PREFAB_PATH = $"{LEVELS_PATH}_BaseLevel.prefab";
		private const string SHAPES_PATH = "Assets/_Main/Prefabs/Shapes";

		#endregion

		[MenuItem("Jelly Math/Level Editor", false, 0)]
		private static void ShowWindow()
		{
			window = GetWindow<LevelEditor>();
			window.titleContent = new GUIContent("Level Editor");
			window.minSize = new Vector2(750, 750);
			window.Show();
		}

		private void CreateGUI()
		{
			InitElements();

			InitMainTabs();
			InitGridTabs();
			InitDeckTabs();

			LoadShapes();

			SetupElements();

			EditorCoroutineUtility.StartCoroutine(Wait(), this);
			return;

			IEnumerator Wait()
			{
				yield return null;
				SetupGrid();
			}
		}

		private void InitElements()
		{
			treeAsset.CloneTree(rootVisualElement);

			MainTabsRowVisualElement = rootVisualElement.Q<VisualElement>(nameof(MainTabsRowVisualElement));
			MainVisualElement = rootVisualElement.Q<VisualElement>(nameof(MainVisualElement));

			MainGridVisualElement = rootVisualElement.Q<VisualElement>(nameof(MainGridVisualElement));
			MainDeckVisualElement = rootVisualElement.Q<VisualElement>(nameof(MainDeckVisualElement));

			// Deck
			DeckVisualElement = rootVisualElement.Q<VisualElement>(nameof(DeckVisualElement));
			DeckTabsRowVisualElement = rootVisualElement.Q<VisualElement>(nameof(DeckTabsRowVisualElement));
			btn_AddDeckTab = rootVisualElement.Q<Button>(nameof(btn_AddDeckTab));
			btn_AddDeckTab.clickable.clicked += AddDeckTab;

			listViewShapes = rootVisualElement.Q<ListView>(nameof(listViewShapes));

			txt_DeckX = rootVisualElement.Q<TextField>(nameof(txt_DeckX));
			txt_DeckY = rootVisualElement.Q<TextField>(nameof(txt_DeckY));
			btn_DeckSetup = rootVisualElement.Q<Button>(nameof(btn_DeckSetup));
			btn_DeckSetup.clickable.clicked += SetupDeckGrid;
			txt_ShapeValue = rootVisualElement.Q<TextField>(nameof(txt_ShapeValue));

			// Grid
			listViewHolders = rootVisualElement.Q<ListView>(nameof(listViewHolders));

			ColorPicker = rootVisualElement.Q<ColorField>("the-color-field");
			ColorPicker.value = Color.black;
			ColorPicker.showAlpha = false;
			ColorPicker.RegisterValueChangedCallback(evt => selectedColor = evt.newValue);

			TabsRowVisualElement = rootVisualElement.Q<VisualElement>(nameof(TabsRowVisualElement));
			btn_AddTab = rootVisualElement.Q<Button>(nameof(btn_AddTab));
			btn_AddTab.clickable.clicked += AddGridTab;

			GridVisualElement = rootVisualElement.Q<VisualElement>(nameof(GridVisualElement));

			txt_LevelTime = rootVisualElement.Q<TextField>(nameof(txt_LevelTime));
			txt_LevelNo = rootVisualElement.Q<TextField>(nameof(txt_LevelNo));
			btn_Save = rootVisualElement.Q<Button>(nameof(btn_Save));
			btn_Save.clickable.clicked += Save;
		}

		private void SetupElements()
		{
			// Deck
			mainTabs[0].VisualElement.Add(MainDeckVisualElement);
			// Grid
			mainTabs[1].VisualElement.Add(MainGridVisualElement);
		}

		#region Grid

		private static Vector2Int cellCount = Grid.Size;
		private List<CellInfo[,]> gridCells = new List<CellInfo[,]>();

		private void SetupGrid()
		{
			gridCells = new List<CellInfo[,]>();
			for (int i = 0; i < gridTabs.Count; i++)
			{
				AddGrid(i);
			}
		}

		private void AddGrid(int tabIndex)
		{
			gridCells.Add(new CellInfo[cellCount.x, cellCount.y]);

			var grid = gridTabs[tabIndex].VisualElement;
			grid.Clear();

			for (int y = 0; y < cellCount.y; y++)
			{
				var row = EditorUtilities.CreateVisualElement<VisualElement>("gridRow");
				grid.Add(row);
				for (int x = 0; x < cellCount.x; x++)
				{
					gridCells[tabIndex][x, y] = new CellInfo();
					var button = EditorUtilities.CreateVisualElement<Button>("cell");
					button.focusable = false;
					gridCells[tabIndex][x, y].Coordinates = new Vector2Int(x, y);
					gridCells[tabIndex][x, y].Color = Color.white;
					gridCells[tabIndex][x, y].Button = button;

					int i1 = tabIndex;
					int x1 = x;
					int y1 = y;
					button.RegisterCallback<MouseDownEvent>(e => OnCellClicked(e, gridCells[i1][x1, y1], i1), TrickleDown.TrickleDown);

					row.Add(button);
				}
			}
		}

		#endregion

		#region Holders

		private void OnCellClicked(IMouseEvent e, CellInfo cell, int tabIndex)
		{
			if (cell.Button is null) return;

			if (e.button.Equals(0))
			{
				OnColorAdded(selectedColor, cell, tabIndex);
			}
			else if (e.button.Equals(1))
			{
				OnColorRemoved(cell);
			}
		}

		private void OnColorAdded(Color color, CellInfo cell, int tabIndex)
		{
			if (cell.Button.style.backgroundColor.Equals(selectedColor)) return;

			cell.Color = selectedColor;
			cell.Button.style.backgroundColor = selectedColor;

			if (colorHolderPair.ContainsKey(color))
			{
				colorHolderPair[color].Cells.Add(cell);
				if (!colorHolderPair[color].Value.Equals(0))
					cell.Button.text = colorHolderPair[color].Value.ToString();
			}
			else
			{
				colorHolderPair.Add(color, new GridNodeInfo(cell, tabIndex));
				AddColorHolderPair(color);
			}
		}

		private void AddColorHolderPair(Color color)
		{
			var pair = EditorUtilities.CreateVisualElement<VisualElement>("holder");
			pair.style.backgroundColor = color;
			var inputField = EditorUtilities.CreateVisualElement<IntegerField>();
			inputField.maxLength = 2;
			colorHolderPair[color].IntegerField = inputField;
			inputField.RegisterValueChangedCallback(evt =>
			{
				var holderPair = colorHolderPair[color];
				holderPair.Value = evt.newValue;
				foreach (var btn in holderPair.Cells)
				{
					if (!holderPair.Value.Equals(0))
						btn.Button.text = holderPair.Value.ToString();
				}
			});

			pair.Add(inputField);
			listViewHolders.hierarchy[0].hierarchy[0].hierarchy[0].hierarchy[0].Add(pair);

			listViewHolders.RefreshItems();
		}

		private void OnColorRemoved(CellInfo cell)
		{
			var button = cell.Button;
			if (colorHolderPair.TryGetValue(button.style.backgroundColor.value, out var holder))
			{
				// Weird
				var deletedCell = holder.Cells.Find(x => x.Coordinates.Equals(cell.Coordinates));
				holder.Cells.Remove(deletedCell);
				deletedCell.Button.text = "";
				if (holder.Cells.Count <= 0)
				{
					colorHolderPair.Remove(button.style.backgroundColor.value);
					listViewHolders.hierarchy[0].hierarchy[0].hierarchy[0].hierarchy[0].Remove(holder.IntegerField.parent);
					listViewHolders.RefreshItems();
				}
			}

			button.style.backgroundColor = Color.white;
		}

		#endregion

		#region Tabs

		private struct Tab
		{
			public int Index;
			public VisualElement VisualElement;
			public Button TabButton;

			public Tab(int index, VisualElement visualElement, Button tabButton)
			{
				Index = index;
				VisualElement = visualElement;
				TabButton = tabButton;
			}
		}

		private void SelectTab(int index, IReadOnlyList<Tab> tabs)
		{
			for (var i = 0; i < tabs.Count; i++)
			{
				var tab = tabs[i];
				if (i.Equals(index))
				{
					tab.VisualElement.style.display = DisplayStyle.Flex;
					tab.TabButton.style.backgroundColor = new Color(.25f, .25f, .25f, 1);
					tab.TabButton.style.borderTopWidth = tab.TabButton.style.borderLeftWidth = tab.TabButton.style.borderRightWidth = 2;
					tab.TabButton.style.borderTopColor = tab.TabButton.style.borderLeftColor = tab.TabButton.style.borderRightColor = Color.white;
				}
				else
				{
					tab.VisualElement.style.display = DisplayStyle.None;
					tab.TabButton.style.backgroundColor = Color.grey;
					tab.TabButton.style.borderTopWidth = tab.TabButton.style.borderLeftWidth = tab.TabButton.style.borderRightWidth = 1;
					tab.TabButton.style.borderTopColor = tab.TabButton.style.borderLeftColor = tab.TabButton.style.borderRightColor = Color.black;
				}
			}
		}

		#region MainTabs

		private List<Tab> mainTabs = new List<Tab>();

		private void InitMainTabs()
		{
			AddMainTab();

			SelectTab(0, mainTabs);
		}

		private void AddMainTab()
		{
			var deckButton = EditorUtilities.CreateVisualElement<Button>("tab-button");
			deckButton.focusable = false;
			deckButton.text = "Deck";
			deckButton.clickable.clicked += () => SelectTab(0, mainTabs);
			MainTabsRowVisualElement.Add(deckButton);
			var visualElement1 = EditorUtilities.CreateVisualElement<VisualElement>("main");
			MainVisualElement.Add(visualElement1);
			mainTabs.Add(new Tab(0, visualElement1, deckButton));

			var gridButton = EditorUtilities.CreateVisualElement<Button>("tab-button");
			gridButton.focusable = false;
			gridButton.text = "Grid";
			gridButton.clickable.clicked += () => SelectTab(1, mainTabs);
			MainTabsRowVisualElement.Add(gridButton);
			var visualElement2 = EditorUtilities.CreateVisualElement<VisualElement>("main");
			MainVisualElement.Add(visualElement2);
			mainTabs.Add(new Tab(1, visualElement2, gridButton));
		}

		#endregion

		#region Deck Tabs

		private List<Tab> deckTabs = new List<Tab>();

		private void InitDeckTabs()
		{
			AddDeckTab();

			SelectTab(0, deckTabs);
		}

		private void AddDeckTab()
		{
			int tabCount = deckTabs.Count;

			var button = EditorUtilities.CreateVisualElement<Button>("tab-button");
			button.focusable = false;
			button.text = "Deck " + (tabCount + 1).ToString();
			button.clickable.clicked += () => SelectTab(tabCount, deckTabs);

			DeckTabsRowVisualElement.Add(button);

			var grid = EditorUtilities.CreateVisualElement<VisualElement>("grid");
			DeckVisualElement.Add(grid);

			deckTabs.Add(new Tab(tabCount, grid, button));

			if (hasDeckSetup)
				AddDeckGrid(deckTabs.Count - 1);
		}

		#endregion

		#region Grid Tabs

		private List<Tab> gridTabs = new List<Tab>();

		private void InitGridTabs()
		{
			AddGridTab();

			SelectTab(0, gridTabs);
		}

		private void AddGridTab()
		{
			int tabCount = gridTabs.Count;

			var button = EditorUtilities.CreateVisualElement<Button>("tab-button");
			button.focusable = false;
			button.text = "Stage " + (tabCount + 1).ToString();
			button.clickable.clicked += () => SelectTab(tabCount, gridTabs);

			TabsRowVisualElement.Add(button);

			var grid = EditorUtilities.CreateVisualElement<VisualElement>("grid");
			GridVisualElement.Add(grid);

			gridTabs.Add(new Tab(tabCount, grid, button));
			AddGrid(gridTabs.Count - 1);
		}

		#endregion

		#endregion

		#region Shapes

		private class Shapes
		{
			public Shape Shape;

			public Shapes(Shape shape)
			{
				Shape = shape;
			}
		}

		private List<Shapes> shapes;
		private Shape selectedShape;

		private void LoadShapes()
		{
			var shapeObjects = EditorUtilities.LoadAllAssetsFromPath<Object>(SHAPES_PATH).ToArray();
			var shapePrefabs = EditorUtilities.LoadAllAssetsFromPath<Shape>(SHAPES_PATH);
			shapes = new List<Shapes>();
			foreach (var shape in shapePrefabs)
			{
				if (shape.name.Equals("_BaseShape")) continue;
				shapes.Add(new Shapes(shape));
			}

			listViewShapes.makeItem = MakeItem;
			listViewShapes.bindItem = BindItem;
			listViewShapes.itemsSource = shapes;
			return;

			VisualElement MakeItem() => EditorUtilities.CreateVisualElement<RadioButton>("radio");

			void BindItem(VisualElement element, int i)
			{
				var radio = (RadioButton)element;

				radio.name = "Shape_" + i;
				radio.label = shapes[i].Shape.name;
				LevelEditorUtilities.LoadAssetPreview(radio, shapeObjects[i], this);

				radio.RegisterValueChangedCallback(evt => SelectShape(evt.newValue, shapes[i].Shape));
			}
		}

		private void SelectShape(bool selected, Shape shape)
		{
			if (!selected) return;
			selectedShape = shape;
			direction = Directions.Up;
		}

		#endregion

		#region Deck

		private bool hasDeckSetup = false;
		private List<DeckCellInfo[,]> deckCells;
		private Dictionary<string, List<Vector2Int>> shapePairs = new Dictionary<string, List<Vector2Int>>();

		private void SetupDeckGrid()
		{
			deckCells = new List<DeckCellInfo[,]>();
			for (int i = 0; i < deckTabs.Count; i++)
			{
				AddDeckGrid(i);
			}

			hasDeckSetup = true;
		}

		private void AddDeckGrid(int tabIndex)
		{
			var cellCountX = int.Parse(txt_DeckX.value);
			var cellCountY = int.Parse(txt_DeckY.value);
			deckCells.Add(new DeckCellInfo[cellCountX, cellCountY]);

			var grid = deckTabs[tabIndex].VisualElement;
			grid.Clear();

			for (int y = 0; y < cellCountY; y++)
			{
				var row = EditorUtilities.CreateVisualElement<VisualElement>("gridRow");
				grid.Add(row);
				for (int x = 0; x < cellCountX; x++)
				{
					deckCells[tabIndex][x, y] = new DeckCellInfo();
					var button = EditorUtilities.CreateVisualElement<Button>("cell");
					button.focusable = false;
					int _tabIndex = tabIndex;
					int x1 = x;
					int y1 = y;

					button.RegisterCallback<PointerDownEvent>(e => Delete(e.clickCount, _tabIndex, x1, y1), TrickleDown.TrickleDown);
					button.RegisterCallback<MouseDownEvent>(e => OnClickedDeckGrid(e, _tabIndex, x1, y1), TrickleDown.TrickleDown);
					button.RegisterCallback<MouseEnterEvent>(e => Recolor(_tabIndex, x1, y1));
					button.RegisterCallback<MouseLeaveEvent>(e => ClearShape(_tabIndex));

					row.Add(button);
				}
			}
		}

		private void OnClickedDeckGrid(IMouseEvent e, int tabIndex, int x, int y)
		{
			// Left click - Place
			if (e.button == 0)
			{
				TryToPlace(tabIndex, x, y);
			}
			else if (e.button == 1) // Right Click - Rotate
			{
				direction = direction switch
				{
					Directions.Up => Directions.Left,
					Directions.Left => Directions.Down,
					Directions.Down => Directions.Right,
					Directions.Right => Directions.Up,
					_ => direction
				};

				ClearShape(tabIndex);
				Recolor(tabIndex, x, y);
			}
		}

		private void TryToPlace(int tabIndex, int x, int y)
		{
			var count = selectedShape.Detectors.Length;
			int howMany = 0;
			for (int i = 0; i < count; i++)
			{
				int x1 = GetDeckGridX(x, i);
				int y1 = GetDeckGridY(y, i);
				if (x1 >= 0 && x1 < cellCount.x && y1 >= 0 && y1 < cellCount.y)
				{
					if (!deckCells[tabIndex][x1, y1].Shape)
					{
						howMany++;
					}
				}
			}

			if (howMany != count) return;

			var color = UnityEngine.Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
			var id = GenerateID();
			for (int i = 0; i < count; i++)
			{
				int x1 = GetDeckGridX(x, i);
				int y1 = GetDeckGridY(y, i);
				if (x1 >= 0 && x1 < cellCount.x && y1 >= 0 && y1 < cellCount.y)
				{
					deckTabs[tabIndex].VisualElement.ElementAt(y1).ElementAt(x1).style.backgroundColor = color;
					var btn = (Button)deckTabs[tabIndex].VisualElement.ElementAt(y1).ElementAt(x1);
					btn.text = txt_ShapeValue.value;
					deckCells[tabIndex][x1, y1].Shape = selectedShape;
					deckCells[tabIndex][x1, y1].Id = id;
					deckCells[tabIndex][x1, y1].Direction = direction;
					deckCells[tabIndex][x1, y1].Value = int.Parse(txt_ShapeValue.value);

					if (!shapePairs.ContainsKey(id))
						shapePairs.Add(id, new List<Vector2Int>());
					shapePairs[id].Add(new Vector2Int(x1, y1));
				}
			}
		}

		private void ClearShape(int tabIndex)
		{
			for (int x = 0; x < deckCells[tabIndex].GetLength(0); x++)
			{
				for (int y = 0; y < deckCells[tabIndex].GetLength(1); y++)
				{
					if (!deckCells[tabIndex][x, y].Shape)
						deckTabs[tabIndex].VisualElement.ElementAt(y).ElementAt(x).style.backgroundColor = Color.white;
				}
			}
		}

		private void Recolor(int tabIndex, int x, int y)
		{
			if (selectedShape)
			{
				var count = selectedShape.Detectors.Length;
				int howMany = 0;
				for (int i = 0; i < count; i++)
				{
					int x1 = GetDeckGridX(x, i);
					int y1 = GetDeckGridY(y, i);
					if (x1 >= 0 && x1 < deckCells[tabIndex].GetLength(0) && y1 >= 0 && y1 < deckCells[tabIndex].GetLength(1))
					{
						if (!deckCells[tabIndex][x1, y1].Shape)
						{
							howMany++;
							deckTabs[tabIndex].VisualElement.ElementAt(y1).ElementAt(x1).style.backgroundColor = Color.green;
						}
						else
						{
							if (!deckCells[tabIndex][x1, y1].Shape)
								deckTabs[tabIndex].VisualElement.ElementAt(y1).ElementAt(x1).style.backgroundColor = Color.red;
						}
					}
				}

				if (howMany != count)
				{
					for (int i = 0; i < count; i++)
					{
						int x1 = GetDeckGridX(x, i);
						int y1 = GetDeckGridY(y, i);
						if (x1 >= 0 && x1 < deckCells[tabIndex].GetLength(0) && y1 >= 0 && y1 < deckCells[tabIndex].GetLength(1))
						{
							if (!deckCells[tabIndex][x1, y1].Shape)
								deckTabs[tabIndex].VisualElement.ElementAt(y1).ElementAt(x1).style.backgroundColor = Color.red;
						}
					}
				}
			}
		}

		private void Delete(int clickCount, int tabIndex, int x, int y)
		{
			if (clickCount <= 1) return;

			var width = deckCells[tabIndex].GetLength(0);
			var height = deckCells[tabIndex].GetLength(1);
			var grid = deckCells[tabIndex][x, y];
			for (int i = 0; i < width; i++)
			{
				for (int j = 0; j < height; j++)
				{
					if (deckCells[tabIndex][i, j].Id == grid.Id)
						EditorCoroutineUtility.StartCoroutine(DeleteCoroutine(tabIndex, i, j), this);
				}
			}
		}

		private IEnumerator DeleteCoroutine(int tabIndex, int x1, int y1)
		{
			yield return new WaitForSeconds(0.1f);

			deckCells[tabIndex][x1, y1].Shape = null;
			deckCells[tabIndex][x1, y1].Id = null;
			deckCells[tabIndex][x1, y1].Value = 0;
			deckTabs[tabIndex].VisualElement.ElementAt(y1).ElementAt(x1).style.backgroundColor = Color.white;
			var btn = (Button)deckTabs[tabIndex].VisualElement.ElementAt(y1).ElementAt(x1);
			btn.text = "";
		}

		private int GetDeckGridX(int x, int i)
		{
			return direction switch
			{
				Directions.Up => x - -selectedShape.Detectors[i].Coordinates.y,
				Directions.Right => x - -selectedShape.Detectors[i].Coordinates.x,
				Directions.Left => x - selectedShape.Detectors[i].Coordinates.x,
				Directions.Down => x - selectedShape.Detectors[i].Coordinates.y,
				_ => throw new ArgumentOutOfRangeException()
			};
		}

		private int GetDeckGridY(int y, int i)
		{
			return direction switch
			{
				Directions.Up => y - -selectedShape.Detectors[i].Coordinates.x,
				Directions.Right => y - -selectedShape.Detectors[i].Coordinates.y,
				Directions.Left => y - -selectedShape.Detectors[i].Coordinates.y,
				Directions.Down => y - selectedShape.Detectors[i].Coordinates.x,
				_ => throw new ArgumentOutOfRangeException()
			};
		}

		private string GenerateID()
		{
			return $"{DateTime.Now.Ticks / 10 % 1000000000:d9}";
		}

		#endregion

		#region Save

		private GameObject levelBasePrefab;

		private void Save()
		{
			if (!int.TryParse(txt_LevelNo.value, out levelNo)) return;

			var source = AssetDatabase.LoadAssetAtPath<GameObject>(LEVEL_BASE_PREFAB_PATH);
			// Need to instantiate this prefab to the scene in order to create a variant
			levelBasePrefab = (GameObject)PrefabUtility.InstantiatePrefab(source);
			var levelBase = levelBasePrefab.GetComponent<Level>();

			EditorUtility.SetDirty(levelBasePrefab);
			EditorSceneManager.MarkSceneDirty(levelBasePrefab.scene);

			//
			levelBase.GridManager.SetupGrids(colorHolderPair, gridCells.Count);
			levelBase.DeckManager.SetupDecks(deckCells, shapePairs);
			if (!txt_LevelTime.value.Equals(""))
				levelBase.Timer = int.Parse(txt_LevelTime.value);
			//

			var levelPath = $"{LEVELS_PATH}Level_{levelNo:000}.prefab";
			levelPath = AssetDatabase.GenerateUniqueAssetPath(levelPath);
			PrefabUtility.SaveAsPrefabAsset(levelBasePrefab, levelPath);

			EditorUtility.ClearDirty(levelBasePrefab);

			AssetDatabase.Refresh();
			Debug.Log($"{levelPath} has saved!");

			DestroyImmediate(levelBasePrefab);
			levelBasePrefab = null;
		}

		#endregion
	}
}