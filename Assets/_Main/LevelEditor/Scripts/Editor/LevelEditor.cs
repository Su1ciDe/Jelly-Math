using System.Collections;
using System.Collections.Generic;
using Fiber.LevelSystem;
using Fiber.Utilities;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace LevelEditor
{
	public class LevelEditor : EditorWindow
	{
		private static LevelEditor window;

		#region Elements

		[SerializeField] private VisualTreeAsset treeAsset;

		private ListView listViewHolders;

		private ColorField ColorPicker;

		private VisualElement TabsRowVisualElement;
		private Button btn_AddTab;

		private VisualElement GridVisualElement;

		private Button btn_Save;
		private TextField txt_LevelNo;

		#endregion

		private Color selectedColor = Color.black;
		private Dictionary<Color, GridNodeInfo> colorHolderPair = new Dictionary<Color, GridNodeInfo>();

		private int levelNo;

		private const float CELL_SIZE = 50;
		private const float PIECE_SIZE = 1;

		#region Paths

		private const string LEVELS_PATH = "Assets/_Main/Prefabs/Levels/";
		private static readonly string LEVEL_BASE_PREFAB_PATH = $"{LEVELS_PATH}_BaseLevel.prefab";

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
			// EditorSceneManager.OpenScene(EDITOR_SCENE_PATH);

			InitElements();
			InitTabs();

			EditorCoroutineUtility.StartCoroutineOwnerless(Wait());
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

			listViewHolders = rootVisualElement.Q<ListView>(nameof(listViewHolders));

			ColorPicker = rootVisualElement.Q<ColorField>("the-color-field");
			ColorPicker.value = Color.black;
			ColorPicker.showAlpha = false;
			ColorPicker.RegisterValueChangedCallback(evt => selectedColor = evt.newValue);

			TabsRowVisualElement = rootVisualElement.Q<VisualElement>(nameof(TabsRowVisualElement));
			btn_AddTab = rootVisualElement.Q<Button>(nameof(btn_AddTab));
			btn_AddTab.clickable.clicked += AddTab;

			GridVisualElement = rootVisualElement.Q<VisualElement>(nameof(GridVisualElement));

			txt_LevelNo = rootVisualElement.Q<TextField>(nameof(txt_LevelNo));
			btn_Save = rootVisualElement.Q<Button>(nameof(btn_Save));
			btn_Save.clickable.clicked += Save;
		}

		#region Grid

		private static Vector2Int cellCount = new Vector2Int(10, 10);
		private List<CellInfo[,]> cells = new List<CellInfo[,]>();

		private void SetupGrid()
		{
			// size window
			const float margin = 10;
			var sizeX = (cellCount.x + 1) * (CELL_SIZE + margin);
			var sizeY = (cellCount.y + 1) * (CELL_SIZE + margin);
			window.minSize = new Vector2(sizeX + sizeX * 50 / 100f, sizeY + sizeY * 30 / 100f);

			cells = new List<CellInfo[,]>();
			for (int i = 0; i < tabs.Count; i++)
			{
				AddGrid(i);
			}
		}

		private void AddGrid(int tabIndex)
		{
			cells.Add(new CellInfo[cellCount.x, cellCount.y]);

			var grid = tabs[tabIndex].Grid;
			grid.Clear();

			for (int y = 0; y < cellCount.y; y++)
			{
				var row = EditorUtilities.CreateVisualElement<VisualElement>("gridRow");
				grid.Add(row);
				for (int x = 0; x < cellCount.x; x++)
				{
					var button = EditorUtilities.CreateVisualElement<Button>("cell");
					button.focusable = false;
					cells[tabIndex][x, y].Coordinates = new Vector2Int(x, y);
					cells[tabIndex][x, y].Color = Color.white;
					cells[tabIndex][x, y].Button = button;

					int i1 = tabIndex;
					int x1 = x;
					int y1 = y;
					button.RegisterCallback<MouseDownEvent>(e => OnCellClicked(e, cells[i1][x1, y1]), TrickleDown.TrickleDown);

					// button.RegisterCallback<PointerDownEvent>(e => Delete(e.clickCount, i1, x1, y1), TrickleDown.TrickleDown);
					// button.RegisterCallback<MouseEnterEvent>(e => ReColor(i1, x1, y1));
					// button.RegisterCallback<MouseLeaveEvent>(e => Clear(i1));

					row.Add(button);
				}
			}
		}

		#endregion

		#region Holders

		private void OnCellClicked(IMouseEvent e, CellInfo cell)
		{
			if (cell.Button is null) return;

			if (e.button.Equals(0))
			{
				OnColorAdded(selectedColor, cell);
			}
			else if (e.button.Equals(1))
			{
				OnColorRemoved(cell);
			}
		}

		private void OnColorAdded(Color color, CellInfo cell)
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
				colorHolderPair.Add(color, new GridNodeInfo(cell));
				AddColorHolderPair(color);
			}
		}

		private void AddColorHolderPair(Color color)
		{
			var pair = EditorUtilities.CreateVisualElement<VisualElement>("holder");
			pair.style.backgroundColor = color;
			var inputField = EditorUtilities.CreateVisualElement<IntegerField>();
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
			listViewHolders.hierarchy.Add(pair);
		}

		private void OnColorRemoved(CellInfo cell)
		{
			var button = cell.Button;
			if (colorHolderPair.TryGetValue(button.style.backgroundColor.value, out var holder))
			{
				// Weird
				var deletedCell = holder.Cells.Find(x => x.Coordinates.Equals(cell.Coordinates));
				holder.Cells.Remove(deletedCell);
				if (holder.Cells.Count <= 0)
				{
					colorHolderPair.Remove(button.style.backgroundColor.value);
					listViewHolders.hierarchy.Remove(holder.IntegerField.parent);
				}
			}

			button.style.backgroundColor = Color.white;
		}

		#endregion

		#region Tabs

		private struct Tab
		{
			public int Index;
			public VisualElement Grid;
			public Button TabButton;

			public Tab(int index, VisualElement grid, Button tabButton)
			{
				Index = index;
				Grid = grid;
				TabButton = tabButton;
			}
		}

		private List<Tab> tabs = new List<Tab>();

		private void InitTabs()
		{
			AddTab();

			SelectTab(0);
		}

		private void AddTab()
		{
			int tabCount = tabs.Count;

			var button = EditorUtilities.CreateVisualElement<Button>("tab-button");
			button.focusable = false;
			button.text = "Stage " + (tabCount + 1).ToString();
			button.clickable.clicked += () => SelectTab(tabCount);

			TabsRowVisualElement.Add(button);

			var grid = EditorUtilities.CreateVisualElement<VisualElement>("grid");
			GridVisualElement.Add(grid);

			tabs.Add(new Tab(tabCount, grid, button));
			AddGrid(tabs.Count - 1);
		}

		private void SelectTab(int index)
		{
			for (var i = 0; i < tabs.Count; i++)
			{
				var tab = tabs[i];
				if (i.Equals(index))
				{
					tab.Grid.style.display = DisplayStyle.Flex;
					tab.TabButton.style.backgroundColor = new Color(.25f, .25f, .25f, 1);
					tab.TabButton.style.borderTopWidth = tab.TabButton.style.borderLeftWidth = tab.TabButton.style.borderRightWidth = 2;
					tab.TabButton.style.borderTopColor = tab.TabButton.style.borderLeftColor = tab.TabButton.style.borderRightColor = Color.white;
				}
				else
				{
					tab.Grid.style.display = DisplayStyle.None;
					tab.TabButton.style.backgroundColor = Color.grey;
					tab.TabButton.style.borderTopWidth = tab.TabButton.style.borderLeftWidth = tab.TabButton.style.borderRightWidth = 1;
					tab.TabButton.style.borderTopColor = tab.TabButton.style.borderLeftColor = tab.TabButton.style.borderRightColor = Color.black;
				}
			}
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
			levelBase.SetupLevel(colorHolderPair, cells.Count);
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