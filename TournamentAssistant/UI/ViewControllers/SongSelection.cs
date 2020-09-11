﻿#pragma warning disable 0649
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.ViewControllers;
using HMUI;
using System;
using System.Collections.Generic;
using System.Linq;
using TournamentAssistant.UI.CustomListItems;
using TournamentAssistantShared.Models;

namespace TournamentAssistant.UI.ViewControllers
{
    [HotReload(@"C:\Users\Moon\source\repos\TournamentAssistant\TournamentAssistant\UI\Views\SongSelection.bsml")]
    [ViewDefinition("TournamentAssistant.UI.Views.SongSelection.bsml")]
    class SongSelection : BSMLAutomaticViewController
    {
        //public override string ResourceName => $"TournamentAssistant.UI.Views.{GetType().Name}.bsml";

        public event Action<GameplayParameters> SongSelected;

        [UIComponent("song-list")]
        public CustomCellListTableData songList;

        [UIValue("songs")]
        public List<object> songs = new List<object>();

        protected override void DidActivate(bool firstActivation, ActivationType type)
        {
            base.DidActivate(firstActivation, type);
            songList.tableView.ClearSelection();
        }

        protected override void DidDeactivate(DeactivationType deactivationType)
        {
            base.DidDeactivate(deactivationType);

            if (deactivationType == DeactivationType.RemovedFromHierarchy) DisposeArtTextures();
        }

        public void SetSongs(List<IPreviewBeatmapLevel> songs)
        {
            this.songs.Clear();
            this.songs.AddRange(songs.Select(x => {
                var parameters = new GameplayParameters
                {
                    Beatmap = new Beatmap
                    {
                        LevelId = x.levelID,
                        Characteristic = new Characteristic
                        {
                            SerializedName = "Standard"
                        },
                        Difficulty = TournamentAssistantShared.SharedConstructs.BeatmapDifficulty.ExpertPlus
                    },
                    GameplayModifiers = new TournamentAssistantShared.Models.GameplayModifiers(),
                    PlayerSettings = new TournamentAssistantShared.Models.PlayerSpecificSettings()
                };
                return new SongListItem(parameters);
            }));

            songList?.tableView.ReloadData();
        }

        public void SetSongs(List<GameplayParameters> songs)
        {
            this.songs.Clear();
            this.songs.AddRange(songs.Select(x => new SongListItem(x)));

            songList?.tableView.ReloadData();
        }

        [UIAction("song-selected")]
        private void ServerClicked(TableView sender, SongListItem songListItem)
        {
            SongSelected?.Invoke(songListItem.parameters);
        }

        //We need to dispose all the textures we've created, so... This is the best option I know of
        //Also disposes the textures that would be loaded by scrolling normally in the solo menu, so... Win-win?
        public void DisposeArtTextures() => songs.ForEach(x => (x as SongListItem).Dispose());
    }
}
