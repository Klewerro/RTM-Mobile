using Moq;
using NUnit.Framework;
using Prism.Navigation;
using Rztm.Helpers;
using Rztm.Models;
using Rztm.ViewModels;
using Rztm.Views;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Rztm.UnitTests
{
    public class ListPageViewModelBaseTets
    {
        private Mock<INavigationService> _navigationService;
        private Mock<IDialogService> _dialogService;
        private ListPageViewModelBase _listPageViewModelBase;

        [SetUp]
        public void Setup()
        {
            _navigationService = new Mock<INavigationService>();
            _dialogService = new Mock<IDialogService>();
            _listPageViewModelBase = new ListPageViewModelBase(_navigationService.Object, _dialogService.Object);
        }

        [Test]
        public void SearchForBusStops_WhenOnlyNamesSet_ShouldReturnOnlyMentionedBusStops()
        {
            _listPageViewModelBase.BusStopsAll = new ReadOnlyCollection<BusStop>(new List<BusStop>
            {
                new BusStop { Name = "Rzeszow Lisa Kuli" },
                new BusStop { Name = "Rzeszow notest" },
                new BusStop { Name = "Warszawa Rejtana name" },
                new BusStop { Name = "Rzeszow Rejtest name" },
                new BusStop { Name = "word1 tanarej word2" },
                new BusStop { Name = "RejtanaRzeszowTest" },
                new BusStop { Name = "Rzeszow Wydzial muzyki" },
                new BusStop { Name = "Rzeszow notest" }
            });

            _listPageViewModelBase.SearchText = "Rejtana";
            _listPageViewModelBase.SearchCommand?.Execute(null);

            Assert.AreEqual(2, _listPageViewModelBase.BusStops.Count);
        }

        [Test]
        public void SearchForBusStops_WhenOnlyCustomNamesSet_ShouldReturnOnlyMentionedBusStops()
        {
            _listPageViewModelBase.BusStopsAll = new ReadOnlyCollection<BusStop>(new List<BusStop>
            {
                new BusStop { Name = "a", CustomName = "Rzeszow Lisa Kuli" },
                new BusStop { Name = "b", CustomName = "Rzeszow notest" },
                new BusStop { Name = "c", CustomName = "Warszawa Rejtana name" },
                new BusStop { Name = "d", CustomName = "Rzeszow Rejtest name" },
                new BusStop { Name = "e", CustomName = "word1 tanarej word2" },
                new BusStop { Name = "f", CustomName = "RejtanaRzeszowTest" },
                new BusStop { Name = "g", CustomName = "Rzeszow Wydzial muzyki" },
                new BusStop { Name = "h", CustomName = "Rzeszow notest" }
            });

            _listPageViewModelBase.SearchText = "Rejtana";
            _listPageViewModelBase.SearchCommand?.Execute(null);

            Assert.AreEqual(2, _listPageViewModelBase.BusStops.Count);
        }

        [Test]
        public void SearchForBusStops_WhenNameAndCustomNamesSet_ShouldReturnOnlyMentionedBusStops()
        {
            _listPageViewModelBase.BusStopsAll = new ReadOnlyCollection<BusStop>(new List<BusStop>
            {
                new BusStop { Name = "Rzeszow Lisa Kuli", CustomName = "a" },
                new BusStop { Name = "b", CustomName = "Rzeszow notest" },
                new BusStop { Name = "Warszawa Rejtana name", CustomName = "c" },
                new BusStop { Name = "d", CustomName = "Rzeszow Rejtest name" },
                new BusStop { Name = "e", CustomName = "word1 tanarej word2" },
                new BusStop { Name = "RejtanaRzeszowTest", CustomName = "f" },
                new BusStop { Name = "g", CustomName = "Rzeszow Wydzial muzyki" },
                new BusStop { Name = "h", CustomName = "Rzeszow notest" }
            });

            _listPageViewModelBase.SearchText = "Rejtana";
            _listPageViewModelBase.SearchCommand?.Execute(null);

            Assert.AreEqual(2, _listPageViewModelBase.BusStops.Count);
        }

    }
}