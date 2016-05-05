/* eslint-env browser */
/*global $, twttr*/

function cleanValues (s) {
  return s.replace(/\W*,\W*/g, ',')
}

var parentIds = [
  'tweet-dinas1',
  'tweet-dinas2',
  'tweet-dinas3',
  'tweet-dinas4',
  'tweet-dinas5',
  'tweet-tidakjelas']

var badgeIds = [
  'count-dinas1',
  'count-dinas2',
  'count-dinas3',
  'count-dinas4',
  'count-dinas5',
  'count-tidakjelas'
]

$(() => {
  var tweetrepo = {}

  $('#selector-buttons .btn').on('click', function (e) {
    $('#selector-buttons .btn').removeClass('active')
    $(this).addClass('active')
  })

  $('#tweet-list').on('click', '.mini-tweet', function () {
    var minitweetid = $(this).attr('id')
    var tweet = tweetrepo[minitweetid]

    if (tweet.done) return

    var wrapperDiv = document.createElement('div')
    wrapperDiv.classList.add('details-wrapper')

    this.appendChild(wrapperDiv)

    var tweetDiv = document.createElement('div')
    tweetDiv.classList.add('tweet-wrapper')

    wrapperDiv.appendChild(tweetDiv)

    twttr.widgets.createTweet(tweet.id, tweetDiv, {
      align: 'center'
    })

    var locationmaker = (q) => {
      var lokasiDiv = document.createElement('div')
      lokasiDiv.classList.add('lokasi-wrapper')

      wrapperDiv.appendChild(lokasiDiv)

      lokasiDiv.innerHTML =
      '<iframe width="500" height="500" frameborder="0" style="border:0" ' +
      'src="https://www.google.com/maps/embed/v1/place?' +
      'key=AIzaSyBoLiNGEbr3gfI_Qu7rQri8lQ0Fgp9TqVw&q=' +
      encodeURIComponent(q) + '" allowfullscreen></iframe>'
    }

    /* if (tweet.koordinat && tweet.koordinat.type === 'Point') {
      var c = tweet.koordinat.coordinates
      locationmaker(c[0] + ', ' + c[1])
    } else*/ if (tweet.lokasi) {
      locationmaker(tweet.lokasi + ', Bandung')
    }

    tweet.done = true
  })

  $('#main-form').on('submit', e => {
    e.preventDefault()

    var mainForm = document.getElementById('main-form')

    $('#search-button').addClass('disabled')

    fetch('/getdis?' + $.param({
      keyword: mainForm.keyword.value,
      dinas1: cleanValues(mainForm.dinas1.value),
      dinas2: cleanValues(mainForm.dinas2.value),
      dinas3: cleanValues(mainForm.dinas3.value),
      dinas4: cleanValues(mainForm.dinas4.value),
      dinas5: cleanValues(mainForm.dinas5.value),
      algoritma: mainForm.algoritma.value
    }))
    .then(resp => {
      if (resp.ok) {
        return resp.json()
      } else {
        throw new Error(resp.statusText)
      }
    })
    .then(data => {
      tweetrepo = {}
      parentIds.forEach(id => $('#' + id).empty())
      $('#result').removeClass('no-result')

      var dinasCounts = [0, 0, 0, 0, 0, 0]

      data.forEach((tweet) => {
        dinasCounts[tweet.dinas]++

        var minitweetid = 'minitweet-' + tweet.id

        tweetrepo[minitweetid] = tweet

        $('#' + parentIds[tweet.dinas]).append(
          '<a class="list-group-item mini-tweet" ' +
          'id="' + minitweetid + '" href="#' + minitweetid + '">' +
          '<h5>' + tweet.penulis + '</h5><p>' + tweet.isi + '</p>' +
          '</a>')
      })

      for (var i = 0; i < 6; i++) {
        document.getElementById(badgeIds[i]).textContent = dinasCounts[i]
      }
    })
    .then(() => {
      $('#search-button').removeClass('disabled')
    })
    .catch(error => {
      alert('Terjadi masalah dalam pengambilan tweet. (' + error + ')')
      $('#search-button').removeClass('disabled')
    })

    return false
  })
})
