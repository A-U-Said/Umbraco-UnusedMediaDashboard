angular.module('umbraco.resources').factory('UmDashboardResource', 

    function($http, umbRequestHelper) {

        return {

			startUnusedMediaReport: function() {
				return umbRequestHelper.resourcePromise(
					$http({
						method: "GET",
						url: "backoffice/UMR/UnusedMedia/StartUnusedMediaReport"
					}),
					'Failed to start unused media report'
				);
			},

			getUnusedMediaReportStatus: function() {
				return umbRequestHelper.resourcePromise(
					$http({
						method: "GET",
						url: "backoffice/UMR/UnusedMedia/GetUnusedMediaReportStatus"
					}),
					'Failed to get unused media report status'
				);
			},
			
			getUnusedMediaReportAsCsv: function() {
				return umbRequestHelper.resourcePromise(
					$http({
						method: "GET",
						url: "backoffice/UMR/UnusedMedia/GetUnusedMediaReportAsCsv"
					}),
					'Failed to get unused media report as CSV'
				);
			},
			
			moveItemToRecycling: function(mediaId) {
				return umbRequestHelper.resourcePromise(
					$http({
						method: 'POST',
						url: "backoffice/UMR/UnusedMedia/MoveItemToRecycling",
						params: { mediaId: mediaId }
					}),
					'Failed to get unused move item to Recyle Bin'
				);
			}

		}

    }
); 