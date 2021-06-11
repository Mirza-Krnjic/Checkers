using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Checkers
{
    public class PuzzlesMenuCreator : Singleton<PuzzlesMenuCreator>
    {
        //public Scroll ScrollRef;
        public ToggleGroup PagesContainer;

        [Header("Menu elements: ")]
        public GameObject ScrollItem;
        public GameObject PageItem;
        public GameObject PageIndicatorItem;
        public GameObject RowItem;
        public GameObject PuzzleItem;
        public Transform MenuHolder;

        [Header("Menu options: ")]
        public int PuzzlesInRow = 5;
        public int PuzzlesInColumn = 5;

        [Space]
        public Sprite DefaultSprite;
        public Sprite PassedSprite;
        public Sprite PassedWellSprite;

        public Color32 AvailableTextColor;
        public Color32 NotAvailableTextColor;

        public List<PuzzleItem> Items = new List<PuzzleItem>();

        /// <summary>
        /// Create menu with puzzle items.
        /// </summary>
        public void Create()
        {
            foreach (Transform item in MenuHolder)
            {
                Destroy(item.gameObject);
            }

            List<PuzzleFormat> puzzles = PuzzleController.Instance.Data.Puzzles;
            //List<ScrollItemView> items = new List<ScrollItemView>();
            //List<PageIndicator> indicators = new List<PageIndicator>();
            GameObject scrollItem;
            GameObject curPage = null;
            GameObject curPageIndic;
            GameObject curRow = null;

            int pagesCount = Mathf.CeilToInt(puzzles.Count / PuzzlesInRow * PuzzlesInColumn);
            for (int i = 0; i < puzzles.Count; i++)
            {
                //Create page
                if ((i) % (PuzzlesInRow * PuzzlesInColumn) == 0)
                {
                    scrollItem = Instantiate(ScrollItem, MenuHolder);
                    curPage = Instantiate(PageItem, scrollItem.transform);
                    //items.Add(scrollItem.GetComponent<ScrollItemView>());
                }
                //Create page indicator
                if ((i) % (PuzzlesInRow * PuzzlesInColumn) == 0)
                {
                    curPageIndic = Instantiate(PageIndicatorItem, PagesContainer.transform);
                    //indicators.Add(curPageIndic.GetComponent<PageIndicator>());
                }
                //Create row
                if ((i) % PuzzlesInRow == 0)
                {
                    curRow = Instantiate(RowItem, curPage.transform);
                }

                PuzzleItem puzzleItem = Instantiate(PuzzleItem, curRow.transform).GetComponent<PuzzleItem>();
                PuzzleFormat item = puzzles[i];
                PuzzleResult result = PuzzleController.Instance.GetPuzzleResultById(item.PuzzleId);

                puzzleItem.ElementId  = item.PuzzleId;
                puzzleItem.name = $"PuzzleItem: {i + 1}";
                puzzleItem.IdText.text = (i + 1).ToString();
                puzzleItem.PuzzleActionButton.onClick.AddListener(() => PuzzleController.Instance.StartPuzzle(item));
                Items.Add(puzzleItem);

                UpdatePuzzleItemStatus(item.PuzzleId, result);
            }

            //ScrollRef.InitPageIndicators(indicators.ToArray());
            //ScrollRef.SetupScrollItems(items);
        }

        /// <summary>
        /// Get puzzle menu item sprite depending on puzzle status.
        /// </summary>
        public Sprite GetStatusSprite(PuzzleStatus status)
        {
            switch (status)
            {
                case PuzzleStatus.Passed:
                    return PassedSprite;
                case PuzzleStatus.PassedWell:
                    return PassedWellSprite;
                case PuzzleStatus.Available:
                default:
                    return DefaultSprite;
            }
        }

        /// <summary>
        /// Change puzzle item status in menu.
        /// </summary>
        public void UpdatePuzzleItemStatus(int itemId, PuzzleResult result)
        {
            PuzzleItem puzzleItem = Items.Find(x => x.ElementId == itemId);
            bool isAvailable = result != null && result.Status != PuzzleStatus.NonAvailable;

            puzzleItem.IdText.color = isAvailable ? AvailableTextColor : NotAvailableTextColor;
            puzzleItem.StatusImage.sprite = GetStatusSprite(result != null ? result.Status : PuzzleStatus.NonAvailable);
            puzzleItem.PuzzleActionButton.interactable = isAvailable;
        }
    }
}