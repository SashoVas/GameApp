
let times = 1;
let genreConteiner = $("#genre-conteiner").html()
function AddGenre(event) {
    event.preventDefault();
    genreConteiner = genreConteiner.replace('name="Genres[' + (times - 1) + ']"', 'name="Genres[' + times + ']"')
    $("#main-form").append(genreConteiner);
    times += 1;
}
let commentsPage = 0;
function PostComment(event) {
    event.preventDefault();
    $.post({
        url: 'https://localhost:44385/api/Comments/AddCommentToGame',
        contentType: 'application/json',
        data: JSON.stringify({ gameId: $("#gameId").val(), contents: $("#commentContents").val() }),
        headers:
        {
            "RequestVerificationToken": $("input[name='__RequestVerificationToken']").val()
        },
    });
    $("#commentContents").val("");
}

function LoadMoreComments() {
    $.get({
        url: 'https://localhost:44385/api/Comments/LoadComments/' + commentsPage + '?gameId=' + $("#gameId").val(),
        success: AddComments
    });
    commentsPage++;
}
function escapeHtml(unsafe) {
    return unsafe
        .replace(/&/g, "&amp;")
        .replace(/</g, "&lt;")
        .replace(/>/g, "&gt;")
        .replace(/"/g, "&quot;")
        .replace(/'/g, "&#039;");
}
function AddComments(comments) {
    for (let i = 0; i < comments.comments.length; i++) {
        let currentComment = comments.comments[i];
        let start = `<div class="comment"><div class="user-info"><ul class="d-flex nopadding"><li class="right-div" >` + escapeHtml(currentComment.username) + `</li>`;
        start = start + `<li >Posted on:` + escapeHtml(currentComment.postedOn) + `</li><li >Overall Rating:10</li></ul></div><div class="comment-text"><p >` + escapeHtml(currentComment.contents) + `</p></div></div><hr>`;
        $("#comments").append(start);
    }

}