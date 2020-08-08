var panel = (function () {

    var commentsCreate = function createComments(selector) {

        $("#postId").change(function (e) {

            //clear select box
            $('.js-parentComment-input').val("");

            GetParentComments();
        });

        GetParentComments();

        function GetParentComments() {
            $('.js-parentComment-input').select2({
                multiple: false,
                dir: "rtl",
                ajax: {
                    url: '/Admin/Comments/GetParentComments',
                    width: 'resolve',
                    data: function (params) {
                        return {
                            q: params.term,// search term
                            postId: $("#postId").val()
                        };
                    },
                    processResults: function (data) {
                        return {
                            results: data.items
                        };
                    },
                    minimumInputLength: 2
                }
            });
        }

        $('form').submit(function (e) {
            var validator = $('form').validate();
            //If form was valid
            if (validator.checkForm()) {
                $('#parentComment').val($('.js-parentComment-input').select2('val'));
            } else {
                e.preventDefault();
            }
        });


    }



    return {
        commentsCreate: commentsCreate
    }

})();