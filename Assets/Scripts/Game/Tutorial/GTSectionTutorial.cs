using UnityEngine;
using TMPro;
using DLIFR.Data;
using DLIFR.Entities;
using DLIFR.Props;

namespace DLIFR.Game.Tutorial
{
    public class GTSectionTutorial : GameTutorialSection 
    {
        public Transform hudFuel;
        public Transform hudCoins;
        public Transform hudClock;

        public Transform shopWishList;
        public Transform sellArea;

        public Transform vault;

        public Transform shopSliders;
        public Transform shopSlider1;
        public Transform shopSlider2;
        public Transform shopSlider3;
        public Transform shopRemainder;

        public bool CanSelectCrewmate = false;
        public bool CanWalkTo = false;
        public bool CanGrab = false;
        public bool CanDrop = false;
        public bool CanCloseShop = false;

        private void OnDisable() 
        {
            match.canDoAction = null;
        }
        
        public bool CanDoAction(string action, GameObject obj)
        {
            if(action == "crewmate_select" && CanSelectCrewmate)
            {
                NextPage();
                return true;
            }

            if(action == "crewmate_walk_end")
            {
                NextPage();
                return true;
            }

            if(action == "close_shop" && CanCloseShop)
            {
                NextPage();
                CanCloseShop = false;
                return true;
            }

            if(action == "crewmate_walk_to" && CanWalkTo)
            {
                tutorial.Display(new TutorialPage{ hasText = false });
                CanWalkTo = false;
                return true;
            }

            if(action == "grab" && CanGrab)
            {
                tutorial.Display(new TutorialPage{ hasText = false });
                CanGrab = false;
                return true;
            }

            if(action == "drop" && CanDrop)
            {
                tutorial.Display(new TutorialPage{ hasText = false });
                CanDrop = false;
                return true;
            }

            if(action == "next_day")
            {
                NextPage();
            }

            Debug.Log(action);

            return false;
        }
        
        public override bool OnOpenPage(int page)
        {
            match.canDoAction = CanDoAction;

            switch(page)
            {
                case 0:
                {
                    Crewmate crewmate = GameObject.FindObjectOfType<Crewmate>();

                    tutorial.Display(new TutorialPage{
                        text = "tutorial.boxes.0",
                        shouldPauseGame = true,
                        shouldTimePass = false,
                        shouldSwipe = true,
                        swipeTarget = crewmate.transform,
                        shouldFocusOnto = true,
                        focusOnto = crewmate.transform
                    });

                    CanSelectCrewmate = true;
                }
                return true;

                case 1:
                {
                    tutorial.Display(new TutorialPage{
                        text = "tutorial.boxes.1"
                    });

                    CanSelectCrewmate = false;
                    CanWalkTo = true;
                }
                return true;

                case 2:
                {
                    DeliveryBird bird = match.SpawnBird(match.GetRandom(match.prefabFuelBox));
                
                    tutorial.Display(new TutorialPage{
                        text = "tutorial.boxes.2",
                        shouldSwipe = true,
                        swipeTarget = bird.carrying,
                    });

                    CanWalkTo = false;
                    CanGrab = true;
                }
                return true;   

                case 3:
                {
                    CanGrab = false;

                    tutorial.Display(new TutorialPage{
                        text = "tutorial.boxes.fuelbox.0",
                        shouldPauseGame = false
                    });
                }
                return true;

                case 4:
                {
                    match.shipFuelLevel.value = match.shipFuelMaxLevel - 10f;

                    tutorial.Display(new TutorialPage{
                        text = "tutorial.boxes.fuelbox.1",
                        shouldSwipe = true,
                        swipeTarget = hudFuel,
                        shouldPauseGame = false
                    });
                }
                return true;

                case 5:
                {
                    tutorial.Display(new TutorialPage{
                        text = "tutorial.boxes.fuelbox.2",
                        shouldPauseGame = false
                    });
                }
                return true;

                case 6:
                {
                    tutorial.Display(new TutorialPage{
                        text = "tutorial.boxes.fuelbox.3",
                        shouldSwipe = true,
                        swipeTarget = GameObject.FindObjectOfType<Engine>().transform,
                    });

                    CanDrop = true;
                }
                return true;

                case 7:
                {
                    tutorial.Display(new TutorialPage{
                        text = "tutorial.boxes.fuelbox.4",
                        shouldPauseGame = true
                    });

                    CanDrop = false;
                }
                return true;

                case 8:
                {
                    DeliveryBird bird = match.SpawnBird(match.GetRandom(match.prefabCargoBox[0].prefabs));
                    
                    tutorial.Display(new TutorialPage{
                        text = "tutorial.boxes.cargobox.0",
                        shouldSwipe = true,
                        swipeTarget = bird.carrying
                    });

                    CanGrab = true;
                }
                return true;

                case 9:
                {
                    tutorial.Display(new TutorialPage{
                        text = "tutorial.boxes.cargobox.1",
                        shouldPauseGame = true
                    });

                    CanGrab = false;
                }
                return true;

                case 10:
                {
                    tutorial.Display(new TutorialPage{
                        text = "tutorial.boxes.cargobox.2",
                        shouldPauseGame = true
                    });
                }
                return true;

                case 11:
                {
                    tutorial.Display(new TutorialPage{
                        text = "tutorial.boxes.cargobox.3",
                        shouldPauseGame = true
                    });
                }
                return true;

                case 12:
                {
                    tutorial.Display(new TutorialPage{
                        text = "tutorial.boxes.cargobox.4",
                        swipeTarget = hudCoins,
                        shouldSwipe = true,
                        shouldPauseGame = true
                    });
                }
                return true;

                case 13:
                {
                    tutorial.Display(new TutorialPage{
                        text = "tutorial.boxes.shop.0",
                        shouldPauseGame = true
                    });
                }
                return true;

                case 14:
                {
                    tutorial.Display(new TutorialPage{
                        text = "tutorial.boxes.shop.1",
                        shouldPauseGame = true
                    });
                }
                return true;

                case 15:
                {
                    tutorial.Display(new TutorialPage{
                        text = "tutorial.boxes.shop.2",
                        swipeTarget = shopWishList,
                        shouldSwipe = true,
                        shouldPauseGame = true
                    });
                }
                return true;

                case 16:
                {
                    tutorial.Display(new TutorialPage{
                        text = "tutorial.boxes.shop.3",
                        shouldPauseGame = true
                    });
                }
                return true;
                
                case 17:
                {
                    tutorial.Display(new TutorialPage{
                        text = "tutorial.boxes.shop.4",
                        shouldPauseGame = true
                    });
                }
                return true;

                case 18:
                {
                    match.useOnlyAreas = true;
                    tutorial.Display(new TutorialPage{
                        text = "tutorial.boxes.shop.5",
                        shouldPauseGame = false,
                        shouldSwipe = true,
                        swipeTarget = sellArea
                    });
                }
                return true;

                case 19:
                {
                    CanWalkTo = false;
                    match.useOnlyAreas = false;
                    match.SetSelectedCrewmate(null);

                    match.gameTicks.value = match.ticksPerDay - 250;

                    tutorial.Display(new TutorialPage{
                        text = "tutorial.boxes.shop.6",
                        shouldPauseGame = false,
                        shouldTimePass = true,
                        shouldSwipe = true,
                        swipeTarget = hudClock
                    });
                }
                return true;

                case 20:
                {
                    match.gameTicks.value = match.ticksPerDay;
                    match.OpenShop();

                    tutorial.Display(new TutorialPage{
                        text = "tutorial.shop.0",
                        shouldPauseGame = true
                    });
                }
                return true;

                case 21:
                {
                    tutorial.Display(new TutorialPage{
                        text = "tutorial.shop.1",
                        shouldPauseGame = true
                    });
                }
                return true;

                case 22:
                {
                    tutorial.Display(new TutorialPage{
                        text = "tutorial.shop.2",
                        shouldPauseGame = true,
                        shouldSwipe = true,
                        swipeTarget = shopSliders
                    });
                }
                return true;

                case 23:
                {
                    tutorial.Display(new TutorialPage{
                        text = "tutorial.shop.3",
                        shouldPauseGame = true,
                        shouldSwipe = true,
                        swipeTarget = shopSlider1
                    });
                }
                return true;

                case 24:
                {
                    tutorial.Display(new TutorialPage{
                        text = "tutorial.shop.4",
                        shouldPauseGame = true,
                        shouldSwipe = true,
                        swipeTarget = shopSlider2
                    });
                }
                return true;

                case 25:
                {
                    tutorial.Display(new TutorialPage{
                        text = "tutorial.shop.5",
                        shouldPauseGame = true,
                        shouldSwipe = true,
                        swipeTarget = shopSlider3
                    });
                }
                return true;

                case 26:
                {
                    tutorial.Display(new TutorialPage{
                        text = "tutorial.shop.6",
                        shouldPauseGame = true
                    });
                }
                return true;

                case 27:
                {
                    tutorial.Display(new TutorialPage{
                        text = "tutorial.shop.7",
                        shouldPauseGame = true,
                        shouldSwipe = true,
                        swipeTarget = shopRemainder
                    });
                }
                return true;

                case 28:
                {
                    tutorial.Display(new TutorialPage{
                        text = "tutorial.shop.8"
                    });
                    
                    CanCloseShop = true;
                }
                return true;

                case 29:
                {
                    CanCloseShop = false;

                    tutorial.Display(new TutorialPage{
                        text = "tutorial.shop.9"
                    });
                }
                return true;

                case 30:
                {
                    tutorial.Display(new TutorialPage{
                        text = "tutorial.salary.0",
                        shouldPauseGame = true
                    });
                }
                return true;

                case 31:
                {
                    tutorial.Display(new TutorialPage{
                        text = "tutorial.salary.1",
                        shouldPauseGame = true
                    });
                }
                return true;

                case 32:
                case 33:
                case 34:
                case 35:
                case 36:
                {
                    Crewmate crewmate = GameObject.FindObjectOfType<Crewmate>();

                    tutorial.Display(new TutorialPage{
                        text = $"tutorial.salary.{2 + page - 32}",
                        shouldFocusOnto = true,
                        shouldSwipe = true,
                        swipeTarget = crewmate.transform,
                        focusOnto = crewmate.transform,
                        shouldPauseGame = true,
                        customFocusOffset = new Vector3(0, 1, -3),
                        useCustomFocusOffset = true
                    });

                    match.SetSelectedCrewmate(crewmate);
                }
                return true;
                
                case 37:
                {
                    match.coinCount.value += 25;

                    CanGrab = true;
                    

                    tutorial.Display(new TutorialPage{
                        text = $"tutorial.salary.7",
                        shouldFocusOnto = true,
                        shouldSwipe = true,
                        swipeTarget = vault,
                        focusOnto = vault,

                        shouldPauseGame = false
                    });
                }
                return true;

                case 38:
                {
                    CanGrab = false;

                    tutorial.Display(new TutorialPage{
                        text = "tutorial.salary.8",
                        shouldPauseGame = true
                    });
                }
                return true;

                case 39:
                {
                    tutorial.Display(new TutorialPage{
                        text = "tutorial.salary.9",
                        shouldPauseGame = true
                    });
                }
                return true;

                case 40:
                {
                    tutorial.Display(new TutorialPage{
                        text = "tutorial.end.0",
                        shouldPauseGame = true,
                        focusOnto = match.ship,
                        shouldFocusOnto = true
                    });
                }
                return true;

                case 41:
                {
                    tutorial.Display(new TutorialPage{
                        text = "tutorial.end.1",
                        shouldPauseGame = true,
                        focusOnto = match.ship,
                        shouldFocusOnto = true
                    });
                }
                return true;


                case 42:
                {
                    match.StartGame(false);
                    Destroy(GetComponentInParent<GameTutorial>().gameObject);
                }
                return true;
            }

            return false;
        }

        public override bool CanSkipPage()
        {
            if(CanDrop || CanGrab || CanSelectCrewmate || CanWalkTo || CanCloseShop)
                return false;

            if(!tutorial.currentPage.hasText)
                return false;

            return true;
        }

        public override int GetStartingIndex()
        {
            return 18;
        }
    }
}