using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// Configures the entire museum experience based on the Water Shapes Florida exhibit.
/// This script initializes all skyboxes, navigation paths, interactive elements, and
/// audio components according to the exhibit flowchart.
/// </summary>
public class MuseumSkyboxSetup : MonoBehaviour
{
    [Header("Core Components")]
    [SerializeField] private SkyboxManager skyboxManager;
    [SerializeField] private AudioManager audioManager;
    
    [Header("Configuration")]
    [SerializeField] private bool initializeOnStart = true;
    [SerializeField] private int startingSkyboxIndex = 0; // Default to 1D (Lobby)
    
    private void Awake()
    {
        // Find required components if not assigned
        if (skyboxManager == null)
            skyboxManager = FindObjectOfType<SkyboxManager>();
            
        if (audioManager == null)
            audioManager = FindObjectOfType<AudioManager>();
    }
    
    private void Start()
    {
        if (initializeOnStart && skyboxManager != null)
        {
            SetupMuseumExperience();
            skyboxManager.NavigateToSkybox(startingSkyboxIndex);
        }
    }
    
    /// <summary>
    /// Initializes the museum experience with all skyboxes, navigation paths, and interactive elements.
    /// </summary>
    public void SetupMuseumExperience()
    {
        // Create the list of all skyboxes in the museum experience
        List<SkyboxData> skyboxes = new List<SkyboxData>();
        
        // Add each skybox area according to the experience flowchart
        skyboxes.Add(CreateLobbyArea());            // 1D
        skyboxes.Add(CreateExhibitEntryArea());     // 2A
        
        // Forest and Cave Path
        skyboxes.Add(CreateForestEntranceArea());   // 3A
        skyboxes.Add(CreateLogFocusArea());         // 3B
        skyboxes.Add(CreateCaveEntranceArea());     // 4A
        skyboxes.Add(CreateCavePosition1());        // 4B
        skyboxes.Add(CreateCavePosition2());        // 4C
        skyboxes.Add(CreateCaveOutsideTunnel());    // 4F
        skyboxes.Add(CreateInsideTunnelArea());     // 4E
        skyboxes.Add(CreateTunnelExitArea());       // 4G
        
        // Turtle Wall and Boat Path
        skyboxes.Add(CreateTouchscreenRoomArea());  // 5A
        skyboxes.Add(CreateTurtleWallArea());       // 5B
        skyboxes.Add(CreateTouchscreenArea());      // 5C
        skyboxes.Add(CreateBoatArea());             // 6A
        skyboxes.Add(CreateSpringsArea());          // 6B
        skyboxes.Add(CreateSpringsHistoryArea());   // 6C
        skyboxes.Add(CreateGlassBottomBoatArea());  // 6D
        skyboxes.Add(CreateBoatExitArea());         // 7A
        skyboxes.Add(CreateLoungeEntryArea());      // 7B
        skyboxes.Add(CreateLoungeStandingArea());   // 7C
        skyboxes.Add(CreateLoungeSeatedArea());     // 7D
        
        // Assign the complete list to the SkyboxManager
        skyboxManager.skyboxes = skyboxes;
    }
    
    #region Skybox Creation Methods
    
    /// <summary>
    /// Creates the lobby area (1D) skybox data.
    /// </summary>
    private SkyboxData CreateLobbyArea()
    {
        return new SkyboxData
        {
            skyboxId = "1D",
            displayName = "Museum Entrance",
            skyboxMaterial = Resources.Load<Material>("Skyboxes/1D_Lobby"),
            associatedPrompt = GameObject.Find("1D_EntrancePrompt"),
            entryAudioClip = Resources.Load<AudioClip>("Audio/1D_Entrance_EN"),
            entryAudioClip_Spanish = Resources.Load<AudioClip>("Audio/1D_Entrance_ES"),
            navigationButtons = new NavigationButton[]
            {
                new NavigationButton
                {
                    buttonId = "1D_to_2A",
                    buttonObject = GameObject.Find("Button_1D_to_2A"),
                    targetSkyboxIndex = 1 // 2A Exhibit Entry
                }
            },
            hotspots = new MuseumHotspot[0] // No hotspots in entrance
        };
    }
    
    /// <summary>
    /// Creates the exhibit entry area (2A) skybox data.
    /// </summary>
    private SkyboxData CreateExhibitEntryArea()
    {
        return new SkyboxData
        {
            skyboxId = "2A",
            displayName = "Water Shapes Florida Entrance",
            skyboxMaterial = Resources.Load<Material>("Skyboxes/2A_ExhibitEntry"),
            associatedPrompt = GameObject.Find("2A_EntryPrompt"),
            entryAudioClip = Resources.Load<AudioClip>("Audio/2A_Entry_EN"),
            entryAudioClip_Spanish = Resources.Load<AudioClip>("Audio/2A_Entry_ES"),
            navigationButtons = new NavigationButton[]
            {
                new NavigationButton
                {
                    buttonId = "2A_to_3A",
                    buttonObject = GameObject.Find("Button_2A_to_3A"),
                    targetSkyboxIndex = 2 // 3A Forest Entrance
                },
                new NavigationButton
                {
                    buttonId = "2A_to_5A",
                    buttonObject = GameObject.Find("Button_2A_to_5A"),
                    targetSkyboxIndex = 10 // 5A Touchscreen Room
                }
            },
            hotspots = new MuseumHotspot[]
            {
                new MuseumHotspot
                {
                    hotspotId = "2A_V1",
                    hotspotObject = GameObject.Find("Hotspot_2A_V1"),
                    detailPanel = GameObject.Find("Panel_BatCase"),
                    detailImage = FindOrCreateImage("Panel_BatCase", "Images/batCase"),
                    detailText = FindOrCreateText("Panel_BatCase", "Learn about the bats in Florida"),
                    narrationClip = Resources.Load<AudioClip>("Audio/2A_BatCase_EN"),
                    narrationClip_Spanish = Resources.Load<AudioClip>("Audio/2A_BatCase_ES")
                },
                new MuseumHotspot
                {
                    hotspotId = "2A_V2",
                    hotspotObject = GameObject.Find("Hotspot_2A_V2"),
                    detailPanel = GameObject.Find("Panel_FloridaWater"),
                    detailImage = FindOrCreateImage("Panel_FloridaWater", "Images/floridaWater"),
                    detailText = FindOrCreateText("Panel_FloridaWater", "Learn about water in Florida"),
                    narrationClip = Resources.Load<AudioClip>("Audio/2A_FloridaWater_EN"),
                    narrationClip_Spanish = Resources.Load<AudioClip>("Audio/2A_FloridaWater_ES")
                },
                new MuseumHotspot
                {
                    hotspotId = "2A_V3",
                    hotspotObject = GameObject.Find("Hotspot_2A_V3"),
                    detailPanel = GameObject.Find("Panel_InsectCase"),
                    detailImage = FindOrCreateImage("Panel_InsectCase", "Images/insectCase"),
                    detailText = FindOrCreateText("Panel_InsectCase", "Learn about insects in Florida"),
                    narrationClip = Resources.Load<AudioClip>("Audio/2A_InsectCase_EN"),
                    narrationClip_Spanish = Resources.Load<AudioClip>("Audio/2A_InsectCase_ES")
                }
            }
        };
    }
    
    /// <summary>
    /// Creates the forest entrance area (3A) skybox data.
    /// </summary>
    private SkyboxData CreateForestEntranceArea()
    {
        return new SkyboxData
        {
            skyboxId = "3A",
            displayName = "Forest Entrance",
            skyboxMaterial = Resources.Load<Material>("Skyboxes/3A_ForestEntrance"),
            associatedPrompt = GameObject.Find("3A_ForestPrompt"),
            entryAudioClip = Resources.Load<AudioClip>("Audio/3A_Forest_EN"),
            entryAudioClip_Spanish = Resources.Load<AudioClip>("Audio/3A_Forest_ES"),
            navigationButtons = new NavigationButton[]
            {
                new NavigationButton
                {
                    buttonId = "3A_to_3B",
                    buttonObject = GameObject.Find("Button_3A_to_3B"),
                    targetSkyboxIndex = 3 // 3B Log Focus
                }
            },
            hotspots = new MuseumHotspot[]
            {
                new MuseumHotspot
                {
                    hotspotId = "3A_V1",
                    hotspotObject = GameObject.Find("Hotspot_3A_V1"),
                    detailPanel = GameObject.Find("Panel_HealthyForest"),
                    detailImage = FindOrCreateImage("Panel_HealthyForest", "Images/healthyForest"),
                    detailText = FindOrCreateText("Panel_HealthyForest", "Learn about healthy forests in Florida"),
                    narrationClip = Resources.Load<AudioClip>("Audio/3A_HealthyForest_EN"),
                    narrationClip_Spanish = Resources.Load<AudioClip>("Audio/3A_HealthyForest_ES")
                }
            }
        };
    }
    
    /// <summary>
    /// Creates the log focus area (3B) skybox data.
    /// </summary>
    private SkyboxData CreateLogFocusArea()
    {
        return new SkyboxData
        {
            skyboxId = "3B",
            displayName = "Log Focus",
            skyboxMaterial = Resources.Load<Material>("Skyboxes/3B_LogFocus"),
            entryAudioClip = Resources.Load<AudioClip>("Audio/3B_LogFocus_EN"),
            entryAudioClip_Spanish = Resources.Load<AudioClip>("Audio/3B_LogFocus_ES"),
            navigationButtons = new NavigationButton[]
            {
                new NavigationButton
                {
                    buttonId = "3B_to_4A",
                    buttonObject = GameObject.Find("Button_3B_to_4A"),
                    targetSkyboxIndex = 4 // 4A Cave Entrance
                }
            },
            hotspots = new MuseumHotspot[]
            {
                new MuseumHotspot
                {
                    hotspotId = "3B_V1",
                    hotspotObject = GameObject.Find("Hotspot_3B_V1"),
                    detailPanel = GameObject.Find("Panel_LogDetails"),
                    detailImage = FindOrCreateImage("Panel_LogDetails", "Images/logDetails"),
                    detailText = FindOrCreateText("Panel_LogDetails", "Examine the log and its inhabitants"),
                    narrationClip = Resources.Load<AudioClip>("Audio/3B_LogDetails_EN"),
                    narrationClip_Spanish = Resources.Load<AudioClip>("Audio/3B_LogDetails_ES")
                },
                new MuseumHotspot
                {
                    hotspotId = "3B_V2",
                    hotspotObject = GameObject.Find("Hotspot_3B_V2"),
                    detailPanel = GameObject.Find("Panel_WaterCycle"),
                    detailImage = FindOrCreateImage("Panel_WaterCycle", "Images/waterCycle"),
                    detailText = FindOrCreateText("Panel_WaterCycle", "Learn about the water cycle"),
                    narrationClip = Resources.Load<AudioClip>("Audio/3B_WaterCycle_EN"),
                    narrationClip_Spanish = Resources.Load<AudioClip>("Audio/3B_WaterCycle_ES")
                },
                new MuseumHotspot
                {
                    hotspotId = "3B_V3",
                    hotspotObject = GameObject.Find("Hotspot_3B_V3"),
                    detailPanel = GameObject.Find("Panel_WaterSupportsLife"),
                    detailImage = FindOrCreateImage("Panel_WaterSupportsLife", "Images/waterSupportsLife"),
                    detailText = FindOrCreateText("Panel_WaterSupportsLife", "Learn how water supports life"),
                    narrationClip = Resources.Load<AudioClip>("Audio/3B_WaterSupportsLife_EN"),
                    narrationClip_Spanish = Resources.Load<AudioClip>("Audio/3B_WaterSupportsLife_ES")
                }
            }
        };
    }
    
    /// <summary>
    /// Creates the cave entrance area (4A) skybox data.
    /// </summary>
    private SkyboxData CreateCaveEntranceArea()
    {
        return new SkyboxData
        {
            skyboxId = "4A",
            displayName = "Cave Entrance",
            skyboxMaterial = Resources.Load<Material>("Skyboxes/4A_CaveEntrance"),
            associatedPrompt = GameObject.Find("4A_CavePrompt"),
            entryAudioClip = Resources.Load<AudioClip>("Audio/4A_CaveEntrance_EN"),
            entryAudioClip_Spanish = Resources.Load<AudioClip>("Audio/4A_CaveEntrance_ES"),
            navigationButtons = new NavigationButton[]
            {
                new NavigationButton
                {
                    buttonId = "4A_to_4B",
                    buttonObject = GameObject.Find("Button_4A_to_4B"),
                    targetSkyboxIndex = 5 // 4B Cave Position 1
                },
                new NavigationButton
                {
                    buttonId = "4A_to_2A",
                    buttonObject = GameObject.Find("Button_4A_to_2A"),
                    targetSkyboxIndex = 1 // 2A Exhibit Entry
                }
            },
            hotspots = new MuseumHotspot[]
            {
                new MuseumHotspot
                {
                    hotspotId = "4A_V1",
                    hotspotObject = GameObject.Find("Hotspot_4A_V1"),
                    detailPanel = GameObject.Find("Panel_WaterAboveBelow"),
                    detailImage = FindOrCreateImage("Panel_WaterAboveBelow", "Images/waterAboveBelow"),
                    detailText = FindOrCreateText("Panel_WaterAboveBelow", "Learn about water above and below ground"),
                    narrationClip = Resources.Load<AudioClip>("Audio/4A_WaterAboveBelow_EN"),
                    narrationClip_Spanish = Resources.Load<AudioClip>("Audio/4A_WaterAboveBelow_ES")
                }
            }
        };
    }
    
    /// <summary>
    /// Creates the cave position 1 (4B) skybox data.
    /// </summary>
    private SkyboxData CreateCavePosition1()
    {
        return new SkyboxData
        {
            skyboxId = "4B",
            displayName = "Cave Position 1",
            skyboxMaterial = Resources.Load<Material>("Skyboxes/4B_CavePosition1"),
            entryAudioClip = Resources.Load<AudioClip>("Audio/4B_CavePosition1_EN"),
            entryAudioClip_Spanish = Resources.Load<AudioClip>("Audio/4B_CavePosition1_ES"),
            navigationButtons = new NavigationButton[]
            {
                new NavigationButton
                {
                    buttonId = "4B_to_4C",
                    buttonObject = GameObject.Find("Button_4B_to_4C"),
                    targetSkyboxIndex = 6 // 4C Cave Position 2
                }
            },
            hotspots = new MuseumHotspot[]
            {
                new MuseumHotspot
                {
                    hotspotId = "4B_V1",
                    hotspotObject = GameObject.Find("Hotspot_4B_V1"),
                    detailPanel = GameObject.Find("Panel_GrowthRings"),
                    detailImage = FindOrCreateImage("Panel_GrowthRings", "Images/growthRings"),
                    detailText = FindOrCreateText("Panel_GrowthRings", "Learn about growth rings"),
                    narrationClip = Resources.Load<AudioClip>("Audio/4B_GrowthRings_EN"),
                    narrationClip_Spanish = Resources.Load<AudioClip>("Audio/4B_GrowthRings_ES")
                },
                new MuseumHotspot
                {
                    hotspotId = "4B_V2",
                    hotspotObject = GameObject.Find("Hotspot_4B_V2"),
                    detailPanel = GameObject.Find("Panel_CaveFormations"),
                    detailImage = FindOrCreateImage("Panel_CaveFormations", "Images/caveFormations"),
                    detailText = FindOrCreateText("Panel_CaveFormations", "Learn about cave formations"),
                    narrationClip = Resources.Load<AudioClip>("Audio/4B_CaveFormations_EN"),
                    narrationClip_Spanish = Resources.Load<AudioClip>("Audio/4B_CaveFormations_ES")
                },
                new MuseumHotspot
                {
                    hotspotId = "4B_V3",
                    hotspotObject = GameObject.Find("Hotspot_4B_V3"),
                    detailPanel = GameObject.Find("Panel_FloridasAquifers"),
                    detailImage = FindOrCreateImage("Panel_FloridasAquifers", "Images/floridasAquifers"),
                    detailText = FindOrCreateText("Panel_FloridasAquifers", "Learn about Florida's aquifers"),
                    narrationClip = Resources.Load<AudioClip>("Audio/4B_FloridasAquifers_EN"),
                    narrationClip_Spanish = Resources.Load<AudioClip>("Audio/4B_FloridasAquifers_ES")
                },
                new MuseumHotspot
                {
                    hotspotId = "4B_V4",
                    hotspotObject = GameObject.Find("Hotspot_4B_V4"),
                    detailPanel = GameObject.Find("Panel_CavesNaturalSpaces"),
                    detailImage = FindOrCreateImage("Panel_CavesNaturalSpaces", "Images/cavesNaturalSpaces"),
                    detailText = FindOrCreateText("Panel_CavesNaturalSpaces", "Caves: Natural Spaces in Rock"),
                    narrationClip = Resources.Load<AudioClip>("Audio/4B_CavesNaturalSpaces_EN"),
                    narrationClip_Spanish = Resources.Load<AudioClip>("Audio/4B_CavesNaturalSpaces_ES")
                }
            }
        };
    }
    
    /// <summary>
    /// Creates the cave position 2 (4C) skybox data.
    /// </summary>
    private SkyboxData CreateCavePosition2()
    {
        return new SkyboxData
        {
            skyboxId = "4C",
            displayName = "Cave Position 2",
            skyboxMaterial = Resources.Load<Material>("Skyboxes/4C_CavePosition2"),
            entryAudioClip = Resources.Load<AudioClip>("Audio/4C_CavePosition2_EN"),
            entryAudioClip_Spanish = Resources.Load<AudioClip>("Audio/4C_CavePosition2_ES"),
            navigationButtons = new NavigationButton[]
            {
                new NavigationButton
                {
                    buttonId = "4C_to_4F",
                    buttonObject = GameObject.Find("Button_4C_to_4F"),
                    targetSkyboxIndex = 7 // 4F Cave Outside Tunnel
                },
                new NavigationButton
                {
                    buttonId = "4C_to_4G",
                    buttonObject = GameObject.Find("Button_4C_to_4G"),
                    targetSkyboxIndex = 9 // 4G Tunnel Exit
                }
            },
            hotspots = new MuseumHotspot[]
            {
                new MuseumHotspot
                {
                    hotspotId = "4C_V1",
                    hotspotObject = GameObject.Find("Hotspot_4C_V1"),
                    detailPanel = GameObject.Find("Panel_FossilsFromSea"),
                    detailImage = FindOrCreateImage("Panel_FossilsFromSea", "Images/fossilsFromSea"),
                    detailText = FindOrCreateText("Panel_FossilsFromSea", "Fossils from the sea and land"),
                    narrationClip = Resources.Load<AudioClip>("Audio/4C_FossilsFromSea_EN"),
                    narrationClip_Spanish = Resources.Load<AudioClip>("Audio/4C_FossilsFromSea_ES")
                },
                new MuseumHotspot
                {
                    hotspotId = "4C_V2",
                    hotspotObject = GameObject.Find("Hotspot_4C_V2"),
                    detailPanel = GameObject.Find("Panel_CaveLife"),
                    detailImage = FindOrCreateImage("Panel_CaveLife", "Images/caveLife"),
                    detailText = FindOrCreateText("Panel_CaveLife", "Learn about life in caves"),
                    narrationClip = Resources.Load<AudioClip>("Audio/4C_CaveLife_EN"),
                    narrationClip_Spanish = Resources.Load<AudioClip>("Audio/4C_CaveLife_ES")
                },
                new MuseumHotspot
                {
                    hotspotId = "4C_V3",
                    hotspotObject = GameObject.Find("Hotspot_4C_V3"),
                    detailPanel = GameObject.Find("Panel_PeopleInCaves"),
                    detailImage = FindOrCreateImage("Panel_PeopleInCaves", "Images/peopleInCaves"),
                    detailText = FindOrCreateText("Panel_PeopleInCaves", "Learn about people in caves"),
                    narrationClip = Resources.Load<AudioClip>("Audio/4C_PeopleInCaves_EN"),
                    narrationClip_Spanish = Resources.Load<AudioClip>("Audio/4C_PeopleInCaves_ES")
                }
            }
        };
    }
    
    /// <summary>
    /// Creates the cave outside tunnel (4F) skybox data.
    /// </summary>
    private SkyboxData CreateCaveOutsideTunnel()
    {
        return new SkyboxData
        {
            skyboxId = "4F",
            displayName = "Cave Outside Tunnel",
            skyboxMaterial = Resources.Load<Material>("Skyboxes/4F_CaveOutsideTunnel"),
            associatedPrompt = GameObject.Find("4F_TunnelPrompt"),
            entryAudioClip = Resources.Load<AudioClip>("Audio/4F_OutsideTunnel_EN"),
            entryAudioClip_Spanish = Resources.Load<AudioClip>("Audio/4F_OutsideTunnel_ES"),
            navigationButtons = new NavigationButton[]
            {
                new NavigationButton
                {
                    buttonId = "4F_to_4E",
                    buttonObject = GameObject.Find("Button_4F_to_4E"),
                    targetSkyboxIndex = 8 // 4E Inside Tunnel
                },
                new NavigationButton
                {
                    buttonId = "4F_to_4G",
                    buttonObject = GameObject.Find("Button_4F_to_4G"),
                    targetSkyboxIndex = 9 // 4G Tunnel Exit
                }
            },
            hotspots = new MuseumHotspot[0] // No hotspots here
        };
    }
    
    /// <summary>
    /// Creates the inside tunnel (4E) skybox data.
    /// </summary>
    private SkyboxData CreateInsideTunnelArea()
    {
        return new SkyboxData
        {
            skyboxId = "4E",
            displayName = "Inside Tunnel",
            skyboxMaterial = Resources.Load<Material>("Skyboxes/4E_InsideTunnel"),
            associatedPrompt = GameObject.Find("4E_FossilPrompt"),
            entryAudioClip = Resources.Load<AudioClip>("Audio/4E_InsideTunnel_EN"),
            entryAudioClip_Spanish = Resources.Load<AudioClip>("Audio/4E_InsideTunnel_ES"),
            navigationButtons = new NavigationButton[]
            {
                new NavigationButton
                {
                    buttonId = "4E_to_4G",
                    buttonObject = GameObject.Find("Button_4E_to_4G"),
                    targetSkyboxIndex = 9 // 4G Tunnel Exit
                }
            },
            hotspots = new MuseumHotspot[]
            {
                new MuseumHotspot
                {
                    hotspotId = "4E_V1",
                    hotspotObject = GameObject.Find("Hotspot_4E_V1"),
                    detailPanel = GameObject.Find("Panel_TunnelFossils"),
                    detailImage = FindOrCreateImage("Panel_TunnelFossils", "Images/tunnelFossils"),
                    detailText = FindOrCreateText("Panel_TunnelFossils", "Find fossils inside the tunnel"),
                    narrationClip = Resources.Load<AudioClip>("Audio/4E_TunnelFossils_EN"),
                    narrationClip_Spanish = Resources.Load<AudioClip>("Audio/4E_TunnelFossils_ES")
                }
            }
        };
    }
    
    /// <summary>
    /// Creates the tunnel exit (4G) skybox data.
    /// </summary>
    private SkyboxData CreateTunnelExitArea()
    {
        return new SkyboxData
        {
            skyboxId = "4G",
            displayName = "Tunnel Exit",
            skyboxMaterial = Resources.Load<Material>("Skyboxes/4G_TunnelExit"),
            associatedPrompt = GameObject.Find("4G_ExitPrompt"),
            entryAudioClip = Resources.Load<AudioClip>("Audio/4G_TunnelExit_EN"),
            entryAudioClip_Spanish = Resources.Load<AudioClip>("Audio/4G_TunnelExit_ES"),
            navigationButtons = new NavigationButton[]
            {
                new NavigationButton
                {
                    buttonId = "4G_to_5A",
                    buttonObject = GameObject.Find("Button_4G_to_5A"),
                    targetSkyboxIndex = 10 // 5A Touchscreen Room
                },
                new NavigationButton
                {
                    buttonId = "4G_to_4F",
                    buttonObject = GameObject.Find("Button_4G_to_4F"),
                    targetSkyboxIndex = 7 // 4F Cave Outside Tunnel
                }
            },
            hotspots = new MuseumHotspot[0] // No hotspots here
        };
    }
    
    /// <summary>
    /// Creates the touchscreen room (5A) skybox data.
    /// </summary>
    private SkyboxData CreateTouchscreenRoomArea()
    {
        return new SkyboxData
        {
            skyboxId = "5A",
            displayName = "Touchscreen Room",
            skyboxMaterial = Resources.Load<Material>("Skyboxes/5A_TouchscreenRoom"),
            associatedPrompt = GameObject.Find("5A_RoomPrompt"),
            entryAudioClip = Resources.Load<AudioClip>("Audio/5A_TouchscreenRoom_EN"),
            entryAudioClip_Spanish = Resources.Load<AudioClip>("Audio/5A_TouchscreenRoom_ES"),
            navigationButtons = new NavigationButton[]
            {
                new NavigationButton
                {
                    buttonId = "5A_to_5B",
                    buttonObject = GameObject.Find("Button_5A_to_5B"),
                    targetSkyboxIndex = 11 // 5B Turtle Wall
                },
                new NavigationButton
                {
                    buttonId = "5A_to_5C",
                    buttonObject = GameObject.Find("Button_5A_to_5C"),
                    targetSkyboxIndex = 12 // 5C Touchscreens
                }
            },
            hotspots = new MuseumHotspot[]
            {
                new MuseumHotspot
                {
                    hotspotId = "5A_V1",
                    hotspotObject = GameObject.Find("Hotspot_5A_V1"),
                    detailPanel = GameObject.Find("Panel_TurtleWall"),
                    detailImage = FindOrCreateImage("Panel_TurtleWall", "Images/turtleWall"),
                    detailText = FindOrCreateText("Panel_TurtleWall", "Learn about the turtle wall"),
                    narrationClip = Resources.Load<AudioClip>("Audio/5A_TurtleWall_EN"),
                    narrationClip_Spanish = Resources.Load<AudioClip>("Audio/5A_TurtleWall_ES")
                },
                new MuseumHotspot
                {
                    hotspotId = "5A_V2",
                    hotspotObject = GameObject.Find("Hotspot_5A_V2"),
                    detailPanel = GameObject.Find("Panel_WaterAboveBelowDiorama"),
                    detailImage = FindOrCreateImage("Panel_WaterAboveBelowDiorama", "Images/waterAboveBelowDiorama"),
                    detailText = FindOrCreateText("Panel_WaterAboveBelowDiorama", "Water above and below ground diorama"),
                    narrationClip = Resources.Load<AudioClip>("Audio/5A_WaterAboveBelowDiorama_EN"),
                    narrationClip_Spanish = Resources.Load<AudioClip>("Audio/5A_WaterAboveBelowDiorama_ES")
                }
            }
        };
    }
    
    /// <summary>
    /// Creates the turtle wall (5B) skybox data.
    /// </summary>
    private SkyboxData CreateTurtleWallArea()
    {
        return new SkyboxData
        {
            skyboxId = "5B",
            displayName = "Turtle Wall",
            skyboxMaterial = Resources.Load<Material>("Skyboxes/5B_TurtleWall"),
            entryAudioClip = Resources.Load<AudioClip>("Audio/5B_TurtleWall_EN"),
            entryAudioClip_Spanish = Resources.Load<AudioClip>("Audio/5B_TurtleWall_ES"),
            navigationButtons = new NavigationButton[]
            {
                new NavigationButton
                {
                    buttonId = "5B_to_5C",
                    buttonObject = GameObject.Find("Button_5B_to_5C"),
                    targetSkyboxIndex = 12 // 5C Touchscreens
                },
                new NavigationButton
                {
                    buttonId = "5B_to_6A",
                    buttonObject = GameObject.Find("Button_5B_to_6A"),
                    targetSkyboxIndex = 13 // 6A Boat Area
                }
            },
            hotspots = new MuseumHotspot[]
            {
                new MuseumHotspot
                {
                    hotspotId = "5B_V1",
                    hotspotObject = GameObject.Find("Hotspot_5B_V1"),
                    detailPanel = GameObject.Find("Panel_TurtleShells"),
                    detailImage = FindOrCreateImage("Panel_TurtleShells", "Images/turtleShells"),
                    detailText = FindOrCreateText("Panel_TurtleShells", "Learn about turtle shells"),
                    narrationClip = Resources.Load<AudioClip>("Audio/5B_TurtleShells_EN"),
                    narrationClip_Spanish = Resources.Load<AudioClip>("Audio/5B_TurtleShells_ES")
                },
                new MuseumHotspot
                {
                    hotspotId = "5B_V2",
                    hotspotObject = GameObject.Find("Hotspot_5B_V2"),
                    detailPanel = GameObject.Find("Panel_WaterAboveBelowDetail"),
                    detailImage = FindOrCreateImage("Panel_WaterAboveBelowDetail", "Images/waterAboveBelowDetail"),
                    detailText = FindOrCreateText("Panel_WaterAboveBelowDetail", "Details about water above and below ground"),
                    narrationClip = Resources.Load<AudioClip>("Audio/5B_WaterAboveBelowDetail_EN"),
                    narrationClip_Spanish = Resources.Load<AudioClip>("Audio/5B_WaterAboveBelowDetail_ES")
                }
            }
        };
    }
    
    /// <summary>
    /// Creates the touchscreens (5C) skybox data.
    /// </summary>
    private SkyboxData CreateTouchscreenArea()
    {
        return new SkyboxData
        {
            skyboxId = "5C",
            displayName = "Touchscreens",
            skyboxMaterial = Resources.Load<Material>("Skyboxes/5C_Touchscreens"),
            entryAudioClip = Resources.Load<AudioClip>("Audio/5C_Touchscreens_EN"),
            entryAudioClip_Spanish = Resources.Load<AudioClip>("Audio/5C_Touchscreens_ES"),
            navigationButtons = new NavigationButton[]
            {
                new NavigationButton
                {
                    buttonId = "5C_to_5B",
                    buttonObject = GameObject.Find("Button_5C_to_5B"),
                    targetSkyboxIndex = 11 // 5B Turtle Wall
                },
                new NavigationButton
                {
                    buttonId = "5C_to_6A",
                    buttonObject = GameObject.Find("Button_5C_to_6A"),
                    targetSkyboxIndex = 13 // 6A Boat Area
                }
            },
            hotspots = new MuseumHotspot[]
            {
                new MuseumHotspot
                {
                    hotspotId = "5C_V1",
                    hotspotObject = GameObject.Find("Hotspot_5C_V1"),
                    detailPanel = GameObject.Find("Panel_WaterChallenges"),
                    detailImage = FindOrCreateImage("Panel_WaterChallenges", "Images/waterChallenges"),
                    detailText = FindOrCreateText("Panel_WaterChallenges", "Learn about water challenges"),
                    narrationClip = Resources.Load<AudioClip>("Audio/5C_WaterChallenges_EN"),
                    narrationClip_Spanish = Resources.Load<AudioClip>("Audio/5C_WaterChallenges_ES")
                },
                new MuseumHotspot
                {
                    hotspotId = "5C_V2",
                    hotspotObject = GameObject.Find("Hotspot_5C_V2"),
                    detailPanel = GameObject.Find("Panel_TaxidermyCabinets"),
                    detailImage = FindOrCreateImage("Panel_TaxidermyCabinets", "Images/taxidermyCabinets"),
                    detailText = FindOrCreateText("Panel_TaxidermyCabinets", "Explore the taxidermy cabinets"),
                    narrationClip = Resources.Load<AudioClip>("Audio/5C_TaxidermyCabinets_EN"),
                    narrationClip_Spanish = Resources.Load<AudioClip>("Audio/5C_TaxidermyCabinets_ES")
                }
            }
        };
    }
    
    /// <summary>
    /// Creates the boat area (6A) skybox data.
    /// </summary>
    private SkyboxData CreateBoatArea()
    {
        return new SkyboxData
        {
            skyboxId = "6A",
            displayName = "Boat Area",
            skyboxMaterial = Resources.Load<Material>("Skyboxes/6A_BoatArea"),
            entryAudioClip = Resources.Load<AudioClip>("Audio/6A_BoatArea_EN"),
            entryAudioClip_Spanish = Resources.Load<AudioClip>("Audio/6A_BoatArea_ES"),
            navigationButtons = new NavigationButton[]
            {
                new NavigationButton
                {
                    buttonId = "6A_to_6B",
                    buttonObject = GameObject.Find("Button_6A_to_6B"),
                    targetSkyboxIndex = 14 // 6B Springs Area
                },
                new NavigationButton
                {
                    buttonId = "6A_to_6C",
                    buttonObject = GameObject.Find("Button_6A_to_6C"),
                    targetSkyboxIndex = 15 // 6C Springs History
                },
                new NavigationButton
                {
                    buttonId = "6A_to_6D",
                    buttonObject = GameObject.Find("Button_6A_to_6D"),
                    targetSkyboxIndex = 16 // 6D Glass Bottom Boat
                }
            },
            hotspots = new MuseumHotspot[0] // No hotspots here
        };
    }
    
    /// <summary>
    /// Creates the springs area (6B) skybox data.
    /// </summary>
    private SkyboxData CreateSpringsArea()
    {
        return new SkyboxData
        {
            skyboxId = "6B",
            displayName = "Springs Area",
            skyboxMaterial = Resources.Load<Material>("Skyboxes/6B_SpringsArea"),
            entryAudioClip = Resources.Load<AudioClip>("Audio/6B_SpringsArea_EN"),
            entryAudioClip_Spanish = Resources.Load<AudioClip>("Audio/6B_SpringsArea_ES"),
            navigationButtons = new NavigationButton[]
            {
                new NavigationButton
                {
                    buttonId = "6B_to_6C",
                    buttonObject = GameObject.Find("Button_6B_to_6C"),
                    targetSkyboxIndex = 15 // 6C Springs History
                },
                new NavigationButton
                {
                    buttonId = "6B_to_6D",
                    buttonObject = GameObject.Find("Button_6B_to_6D"),
                    targetSkyboxIndex = 16 // 6D Glass Bottom Boat
                },
                new NavigationButton
                {
                    buttonId = "6B_to_7B",
                    buttonObject = GameObject.Find("Button_6B_to_7B"),
                    targetSkyboxIndex = 18 // 7B Lounge Entry
                }
            },
            hotspots = new MuseumHotspot[]
            {
                new MuseumHotspot
                {
                    hotspotId = "6B_V1",
                    hotspotObject = GameObject.Find("Hotspot_6B_V1"),
                    detailPanel = GameObject.Find("Panel_NaturalWonders"),
                    detailImage = FindOrCreateImage("Panel_NaturalWonders", "Images/naturalWonders"),
                    detailText = FindOrCreateText("Panel_NaturalWonders", "Learn about natural wonders"),
                    narrationClip = Resources.Load<AudioClip>("Audio/6B_NaturalWonders_EN"),
                    narrationClip_Spanish = Resources.Load<AudioClip>("Audio/6B_NaturalWonders_ES")
                },
                new MuseumHotspot
                {
                    hotspotId = "6B_V2",
                    hotspotObject = GameObject.Find("Hotspot_6B_V2"),
                    detailPanel = GameObject.Find("Panel_WorldRecords"),
                    detailImage = FindOrCreateImage("Panel_WorldRecords", "Images/worldRecords"),
                    detailText = FindOrCreateText("Panel_WorldRecords", "World records related to water"),
                    narrationClip = Resources.Load<AudioClip>("Audio/6B_WorldRecords_EN"),
                    narrationClip_Spanish = Resources.Load<AudioClip>("Audio/6B_WorldRecords_ES")
                },
                new MuseumHotspot
                {
                    hotspotId = "6B_V3",
                    hotspotObject = GameObject.Find("Hotspot_6B_V3"),
                    detailPanel = GameObject.Find("Panel_AnatomyOfSpring"),
                    detailImage = FindOrCreateImage("Panel_AnatomyOfSpring", "Images/anatomyOfSpring"),
                    detailText = FindOrCreateText("Panel_AnatomyOfSpring", "Anatomy of a spring"),
                    narrationClip = Resources.Load<AudioClip>("Audio/6B_AnatomyOfSpring_EN"),
                    narrationClip_Spanish = Resources.Load<AudioClip>("Audio/6B_AnatomyOfSpring_ES")
                },
                new MuseumHotspot
                {
                    hotspotId = "6B_V4",
                    hotspotObject = GameObject.Find("Hotspot_6B_V4"),
                    detailPanel = GameObject.Find("Panel_HumanSpringsJourney"),
                    detailImage = FindOrCreateImage("Panel_HumanSpringsJourney", "Images/humanSpringsJourney"),
                    detailText = FindOrCreateText("Panel_HumanSpringsJourney", "The human-springs journey"),
                    narrationClip = Resources.Load<AudioClip>("Audio/6B_HumanSpringsJourney_EN"),
                    narrationClip_Spanish = Resources.Load<AudioClip>("Audio/6B_HumanSpringsJourney_ES")
                },
                new MuseumHotspot
                {
                    hotspotId = "6B_V5",
                    hotspotObject = GameObject.Find("Hotspot_6B_V5"),
                    detailPanel = GameObject.Find("Panel_GlassBottomBoatTour"),
                    detailImage = FindOrCreateImage("Panel_GlassBottomBoatTour", "Images/glassBottomBoatTour"),
                    detailText = FindOrCreateText("Panel_GlassBottomBoatTour", "Glass bottom boat tour"),
                    narrationClip = Resources.Load<AudioClip>("Audio/6B_GlassBottomBoatTour_EN"),
                    narrationClip_Spanish = Resources.Load<AudioClip>("Audio/6B_GlassBottomBoatTour_ES")
                }
            }
        };
    }
    
    /// <summary>
    /// Creates the springs history (6C) skybox data.
    /// </summary>
    private SkyboxData CreateSpringsHistoryArea()
    {
        return new SkyboxData
        {
            skyboxId = "6C",
            displayName = "Springs History",
            skyboxMaterial = Resources.Load<Material>("Skyboxes/6C_SpringsHistory"),
            entryAudioClip = Resources.Load<AudioClip>("Audio/6C_SpringsHistory_EN"),
            entryAudioClip_Spanish = Resources.Load<AudioClip>("Audio/6C_SpringsHistory_ES"),
            navigationButtons = new NavigationButton[]
            {
                new NavigationButton
                {
                    buttonId = "6C_to_6D",
                    buttonObject = GameObject.Find("Button_6C_to_6D"),
                    targetSkyboxIndex = 16 // 6D Glass Bottom Boat
                },
                new NavigationButton
                {
                    buttonId = "6C_to_6B",
                    buttonObject = GameObject.Find("Button_6C_to_6B"),
                    targetSkyboxIndex = 14 // 6B Springs Area
                },
                new NavigationButton
                {
                    buttonId = "6C_to_7A",
                    buttonObject = GameObject.Find("Button_6C_to_7A"),
                    targetSkyboxIndex = 17 // 7A Boat Exit
                }
            },
            hotspots = new MuseumHotspot[]
            {
                new MuseumHotspot
                {
                    hotspotId = "6C_V1",
                    hotspotObject = GameObject.Find("Hotspot_6C_V1"),
                    detailPanel = GameObject.Find("Panel_SpringsAtRisk"),
                    detailImage = FindOrCreateImage("Panel_SpringsAtRisk", "Images/springsAtRisk"),
                    detailText = FindOrCreateText("Panel_SpringsAtRisk", "Learn about springs at risk"),
                    narrationClip = Resources.Load<AudioClip>("Audio/6C_SpringsAtRisk_EN"),
                    narrationClip_Spanish = Resources.Load<AudioClip>("Audio/6C_SpringsAtRisk_ES")
                }
            }
        };
    }
    
    /// <summary>
    /// Creates the glass bottom boat (6D) skybox data.
    /// </summary>
    private SkyboxData CreateGlassBottomBoatArea()
    {
        return new SkyboxData
        {
            skyboxId = "6D",
            displayName = "Glass Bottom Boat",
            skyboxMaterial = Resources.Load<Material>("Skyboxes/6D_GlassBottomBoat"),
            associatedPrompt = GameObject.Find("6D_BoatPrompt"),
            entryAudioClip = Resources.Load<AudioClip>("Audio/6D_GlassBottomBoat_EN"),
            entryAudioClip_Spanish = Resources.Load<AudioClip>("Audio/6D_GlassBottomBoat_ES"),
            navigationButtons = new NavigationButton[]
            {
                new NavigationButton
                {
                    buttonId = "6D_to_7A",
                    buttonObject = GameObject.Find("Button_6D_to_7A"),
                    targetSkyboxIndex = 17 // 7A Boat Exit
                }
            },
            hotspots = new MuseumHotspot[]
            {
                new MuseumHotspot
                {
                    hotspotId = "6D_V1",
                    hotspotObject = GameObject.Find("Hotspot_6D_V1"),
                    detailPanel = GameObject.Find("Panel_BoatWindows"),
                    detailImage = FindOrCreateImage("Panel_BoatWindows", "Images/boatWindows"),
                    detailText = FindOrCreateText("Panel_BoatWindows", "View through the glass bottom boat"),
                    narrationClip = Resources.Load<AudioClip>("Audio/6D_BoatWindows_EN"),
                    narrationClip_Spanish = Resources.Load<AudioClip>("Audio/6D_BoatWindows_ES")
                }
            }
        };
    }
    
    /// <summary>
    /// Creates the boat exit (7A) skybox data.
    /// </summary>
    private SkyboxData CreateBoatExitArea()
    {
        return new SkyboxData
        {
            skyboxId = "7A",
            displayName = "Boat Exit",
            skyboxMaterial = Resources.Load<Material>("Skyboxes/7A_BoatExit"),
            entryAudioClip = Resources.Load<AudioClip>("Audio/7A_BoatExit_EN"),
            entryAudioClip_Spanish = Resources.Load<AudioClip>("Audio/7A_BoatExit_ES"),
            navigationButtons = new NavigationButton[]
            {
                new NavigationButton
                {
                    buttonId = "7A_to_7B",
                    buttonObject = GameObject.Find("Button_7A_to_7B"),
                    targetSkyboxIndex = 18 // 7B Lounge Entry
                },
                new NavigationButton
                {
                    buttonId = "7A_to_6A",
                    buttonObject = GameObject.Find("Button_7A_to_6A"),
                    targetSkyboxIndex = 13 // 6A Boat Area
                }
            },
            hotspots = new MuseumHotspot[0] // No hotspots here
        };
    }
    
    /// <summary>
    /// Creates the lounge entry (7B) skybox data.
    /// </summary>
    private SkyboxData CreateLoungeEntryArea()
    {
        return new SkyboxData
        {
            skyboxId = "7B",
            displayName = "Lounge Entry",
            skyboxMaterial = Resources.Load<Material>("Skyboxes/7B_LoungeEntry"),
            associatedPrompt = GameObject.Find("7B_LoungePrompt"),
            entryAudioClip = Resources.Load<AudioClip>("Audio/7B_LoungeEntry_EN"),
            entryAudioClip_Spanish = Resources.Load<AudioClip>("Audio/7B_LoungeEntry_ES"),
            navigationButtons = new NavigationButton[]
            {
                new NavigationButton
                {
                    buttonId = "7B_to_7C",
                    buttonObject = GameObject.Find("Button_7B_to_7C"),
                    targetSkyboxIndex = 19 // 7C Lounge Standing
                },
                new NavigationButton
                {
                    buttonId = "7B_to_7D",
                    buttonObject = GameObject.Find("Button_7B_to_7D"),
                    targetSkyboxIndex = 20 // 7D Lounge Seated
                }
            },
            hotspots = new MuseumHotspot[]
            {
                new MuseumHotspot
                {
                    hotspotId = "7B_V1",
                    hotspotObject = GameObject.Find("Hotspot_7B_V1"),
                    detailPanel = GameObject.Find("Panel_CommunityShowcase"),
                    detailImage = FindOrCreateImage("Panel_CommunityShowcase", "Images/communityShowcase"),
                    detailText = FindOrCreateText("Panel_CommunityShowcase", "Community showcase"),
                    narrationClip = Resources.Load<AudioClip>("Audio/7B_CommunityShowcase_EN"),
                    narrationClip_Spanish = Resources.Load<AudioClip>("Audio/7B_CommunityShowcase_ES")
                }
            }
        };
    }
    
    /// <summary>
    /// Creates the lounge standing (7C) skybox data.
    /// </summary>
    private SkyboxData CreateLoungeStandingArea()
    {
        return new SkyboxData
        {
            skyboxId = "7C",
            displayName = "Lounge Standing",
            skyboxMaterial = Resources.Load<Material>("Skyboxes/7C_LoungeStanding"),
            entryAudioClip = Resources.Load<AudioClip>("Audio/7C_LoungeStanding_EN"),
            entryAudioClip_Spanish = Resources.Load<AudioClip>("Audio/7C_LoungeStanding_ES"),
            navigationButtons = new NavigationButton[]
            {
                new NavigationButton
                {
                    buttonId = "7C_to_7D",
                    buttonObject = GameObject.Find("Button_7C_to_7D"),
                    targetSkyboxIndex = 20 // 7D Lounge Seated
                },
                new NavigationButton
                {
                    buttonId = "7C_to_END",
                    buttonObject = GameObject.Find("Button_7C_to_END"),
                    targetSkyboxIndex = 0  // 1D Lobby (end of experience)
                }
            },
            hotspots = new MuseumHotspot[]
            {
                new MuseumHotspot
                {
                    hotspotId = "7C_V1",
                    hotspotObject = GameObject.Find("Hotspot_7C_V1"),
                    detailPanel = GameObject.Find("Panel_RescueRehabilitate"),
                    detailImage = FindOrCreateImage("Panel_RescueRehabilitate", "Images/rescueRehabilitate"),
                    detailText = FindOrCreateText("Panel_RescueRehabilitate", "Rescue, rehabilitate, release"),
                    narrationClip = Resources.Load<AudioClip>("Audio/7C_RescueRehabilitate_EN"),
                    narrationClip_Spanish = Resources.Load<AudioClip>("Audio/7C_RescueRehabilitate_ES")
                },
                new MuseumHotspot
                {
                    hotspotId = "7C_V2",
                    hotspotObject = GameObject.Find("Hotspot_7C_V2"),
                    detailPanel = GameObject.Find("Panel_ManateeVideo"),
                    detailImage = FindOrCreateImage("Panel_ManateeVideo", "Images/manateeVideo"),
                    detailText = FindOrCreateText("Panel_ManateeVideo", "Manatee video"),
                    narrationClip = Resources.Load<AudioClip>("Audio/7C_ManateeVideo_EN"),
                    narrationClip_Spanish = Resources.Load<AudioClip>("Audio/7C_ManateeVideo_ES")
                },
                new MuseumHotspot
                {
                    hotspotId = "7C_V3",
                    hotspotObject = GameObject.Find("Hotspot_7C_V3"),
                    detailPanel = GameObject.Find("Panel_LookingAhead"),
                    detailImage = FindOrCreateImage("Panel_LookingAhead", "Images/lookingAhead"),
                    detailText = FindOrCreateText("Panel_LookingAhead", "Looking ahead"),
                    narrationClip = Resources.Load<AudioClip>("Audio/7C_LookingAhead_EN"),
                    narrationClip_Spanish = Resources.Load<AudioClip>("Audio/7C_LookingAhead_ES")
                },
                new MuseumHotspot
                {
                    hotspotId = "7C_V4",
                    hotspotObject = GameObject.Find("Hotspot_7C_V4"),
                    detailPanel = GameObject.Find("Panel_WaterShapesFlorida"),
                    detailImage = FindOrCreateImage("Panel_WaterShapesFlorida", "Images/waterShapesFlorida"),
                    detailText = FindOrCreateText("Panel_WaterShapesFlorida", "Water Shapes Florida summary"),
                    narrationClip = Resources.Load<AudioClip>("Audio/7C_WaterShapesFlorida_EN"),
                    narrationClip_Spanish = Resources.Load<AudioClip>("Audio/7C_WaterShapesFlorida_ES")
                },
                new MuseumHotspot
                {
                    hotspotId = "7C_V5",
                    hotspotObject = GameObject.Find("Hotspot_7C_V5"),
                    detailPanel = GameObject.Find("Panel_PreciousWater"),
                    detailImage = FindOrCreateImage("Panel_PreciousWater", "Images/preciousWater"),
                    detailText = FindOrCreateText("Panel_PreciousWater", "Precious water"),
                    narrationClip = Resources.Load<AudioClip>("Audio/7C_PreciousWater_EN"),
                    narrationClip_Spanish = Resources.Load<AudioClip>("Audio/7C_PreciousWater_ES")
                }
            }
        };
    }
    
    /// <summary>
    /// Creates the lounge seated (7D) skybox data.
    /// </summary>
    private SkyboxData CreateLoungeSeatedArea()
    {
        return new SkyboxData
        {
            skyboxId = "7D",
            displayName = "Lounge Seated",
            skyboxMaterial = Resources.Load<Material>("Skyboxes/7D_LoungeSeated"),
            associatedPrompt = GameObject.Find("7D_EndPrompt"),
            entryAudioClip = Resources.Load<AudioClip>("Audio/7D_LoungeSeated_EN"),
            entryAudioClip_Spanish = Resources.Load<AudioClip>("Audio/7D_LoungeSeated_ES"),
            navigationButtons = new NavigationButton[]
            {
                new NavigationButton
                {
                    buttonId = "7D_to_END",
                    buttonObject = GameObject.Find("Button_7D_to_END"),
                    targetSkyboxIndex = 0 // 1D Lobby (end of experience)
                }
            },
            hotspots = new MuseumHotspot[]
            {
                // Same hotspots as 7C to allow viewing from seated position
                new MuseumHotspot
                {
                    hotspotId = "7D_V1",
                    hotspotObject = GameObject.Find("Hotspot_7D_V1"),
                    detailPanel = GameObject.Find("Panel_RescueRehabilitate"),
                    detailImage = FindOrCreateImage("Panel_RescueRehabilitate", "Images/rescueRehabilitate"),
                    detailText = FindOrCreateText("Panel_RescueRehabilitate", "Rescue, rehabilitate, release"),
                    narrationClip = Resources.Load<AudioClip>("Audio/7D_RescueRehabilitate_EN"),
                    narrationClip_Spanish = Resources.Load<AudioClip>("Audio/7D_RescueRehabilitate_ES")
                },
                new MuseumHotspot
                {
                    hotspotId = "7D_V2",
                    hotspotObject = GameObject.Find("Hotspot_7D_V2"),
                    detailPanel = GameObject.Find("Panel_ManateeVideo"),
                    detailImage = FindOrCreateImage("Panel_ManateeVideo", "Images/manateeVideo"),
                    detailText = FindOrCreateText("Panel_ManateeVideo", "Manatee video"),
                    narrationClip = Resources.Load<AudioClip>("Audio/7D_ManateeVideo_EN"),
                    narrationClip_Spanish = Resources.Load<AudioClip>("Audio/7D_ManateeVideo_ES")
                },
                new MuseumHotspot
                {
                    hotspotId = "7D_V3",
                    hotspotObject = GameObject.Find("Hotspot_7D_V3"),
                    detailPanel = GameObject.Find("Panel_WaterShapesFlorida"),
                    detailImage = FindOrCreateImage("Panel_WaterShapesFlorida", "Images/waterShapesFlorida"),
                    detailText = FindOrCreateText("Panel_WaterShapesFlorida", "Water Shapes Florida summary"),
                    narrationClip = Resources.Load<AudioClip>("Audio/7D_WaterShapesFlorida_EN"),
                    narrationClip_Spanish = Resources.Load<AudioClip>("Audio/7D_WaterShapesFlorida_ES")
                }
            }
        };
    }
    
    #endregion
    
    #region Helper Methods
    
    /// <summary>
    /// Finds an existing Image component or creates one with the specified texture.
    /// </summary>
    private UnityEngine.UI.Image FindOrCreateImage(string panelName, string texturePath)
    {
        GameObject panel = GameObject.Find(panelName);
        if (panel == null)
            return null;
            
        UnityEngine.UI.Image image = panel.GetComponentInChildren<UnityEngine.UI.Image>();
        if (image == null)
        {
            GameObject imageObj = new GameObject("Image");
            imageObj.transform.SetParent(panel.transform, false);
            image = imageObj.AddComponent<UnityEngine.UI.Image>();
        }
        
        // Load the texture if specified
        if (!string.IsNullOrEmpty(texturePath))
        {
            Texture2D texture = Resources.Load<Texture2D>(texturePath);
            if (texture != null)
            {
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f);
                image.sprite = sprite;
            }
        }
        
        return image;
    }
    
    /// <summary>
    /// Finds an existing Text component or creates one with the specified content.
    /// </summary>
    private UnityEngine.UI.Text FindOrCreateText(string panelName, string content)
    {
        GameObject panel = GameObject.Find(panelName);
        if (panel == null)
            return null;
            
        UnityEngine.UI.Text text = panel.GetComponentInChildren<UnityEngine.UI.Text>();
        if (text == null)
        {
            GameObject textObj = new GameObject("Text");
            textObj.transform.SetParent(panel.transform, false);
            text = textObj.AddComponent<UnityEngine.UI.Text>();
            text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        }
        
        if (!string.IsNullOrEmpty(content))
        {
            text.text = content;
        }
        
        return text;
    }
    
    #endregion
}
