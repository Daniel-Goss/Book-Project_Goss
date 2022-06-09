
var sortable = {
    sortBy: 0,
    serachTerm: '',
    URL: '',
    Search() {
        var searchKey = $('#searchBar').val();
        var filter = $('input[name="filters"]:checked').val();

        window.location.href = sortable.URL + searchKey + "/?filter=" + filter;
    },
    sort(sortBy) {
        var isDesc = true;

        const urlParams = new URLSearchParams(window.location.search);

        const isDescOrg = urlParams.get("isDesc");
        const sortByOrg = urlParams.get('sortBy');

        if (sortByOrg == sortBy) {
            if (isDescOrg == "true") {
                isDesc = false;
            }
        }

        window.location.href = sortable.URL + "?sortBy=" + sortBy + "&isDesc=" + isDesc;
    }
}

var apiHandler = {
    GET(url) {
        $.ajax({
            url: url,
            type: 'GET',
            success: function (res) {
                console.log(res)
                location.href = url;
            }
        });
    },
    POST(url, object) {
            $.ajax({
                url: url,
                type: 'GET',
                data: object,
                success: function (res) {
                    console.log(res);
                }
            });
    },
    DELETE(url) {
        $.ajax({
            url: url,
            type: 'GET',
            success: function (res) {
                debugger;
                alert("Gamer");
            }
        });
    }
};