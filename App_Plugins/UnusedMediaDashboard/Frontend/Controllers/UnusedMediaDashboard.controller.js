angular.module("umbraco").controller("UMR.DashboardController", function($scope, UmDashboardResource, $timeout) {

	$scope.filteredMedia = [];

	$scope.unusedMedia = {
		IsProcessingMedia: false,
		Data: [],
		TotalAmountOfMedia: 0,
		TotalUnusedMedia: 0
	};
		
	$scope.startUnusedMediaReport = function () {
		if ($scope.unusedMedia.IsProcessingMedia) {
			return;
		}
		UmDashboardResource.startUnusedMediaReport()
			.then(function () {
				$scope.getUnusedMediaReportStatus();
			});
	};
	
	$scope.getUnusedMediaReportStatus = function () {
		UmDashboardResource.getUnusedMediaReportStatus()
			.then(data => {
				$scope.unusedMedia = data;
				$scope.filteredMedia = data.Data;
				if ($scope.unusedMedia.IsProcessingMedia) {
					$timeout(function () { $scope.getUnusedMediaReportStatus() }, 5000, true);
				}
			});
	};
	
	$scope.downloadUnusedMediaReport = function () {
		UmDashboardResource.getUnusedMediaReportAsCsv()
			.then(data => {
				var data = "data:text/csv;charset=utf-8," + encodeURIComponent(data);
				var downloader = document.createElement('a');
				downloader.setAttribute('href', data);
				downloader.setAttribute('download', 'unusedMediaReport.csv');
				downloader.click();
			});
	}
	
	$scope.moveItemToRecycling = function (mediaId) {
		UmDashboardResource.moveItemToRecycling(mediaId)
			.then(() => {
				$scope.filteredMedia = $scope.filteredMedia.filter(media => media.Id !== mediaId);
			});
	}

});