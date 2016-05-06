<%@ Page Language="C#" %>
<!DOCTYPE html>

<meta name="viewport" content="width=device-width">
<meta charset="utf-8" />
<title>tweetDIS : know.better.</title>
<link rel="stylesheet" href="https://agilajah.github.io/tweetdis/Content/bootstrap.min.css">
<link rel="stylesheet" href="https://agilajah.github.io/tweetdis/Content/main.css">

<div class="blocker"></div>

<a href="#" class="btn btn-primary" id="to-top-link">&#9650;</a>

<div id="primcontent" class="container">

  <div class="row">
    <div class="col-md-6 col-md-offset-3">
      <img src="https://agilajah.github.io/tweetdis/Content/images/logo-tweetdis.png" alt="" class="img-responsive main-logo">
    </div>
  </div>

  <div class="about-link-container">
    <a href="/About.aspx" class="btn btn-info">Tentang Kami</a>
  </div>

  <form action="" class="form-horizontal" id="main-form" class="tab-panel hidden-prese">
    <div class="form-group">
      <div class="col-sm-8">
        <input type="text" class="form-control input-lg" name="keyword" placeholder="#bandung">
      </div>
      <div class="col-sm-2">
        <button id="search-button" type="submit" name="submit" class="btn btn-primary btn-lg btn-block">Cari</button>
      </div>
      <div class="col-sm-2">
        <button type="button" name="options" class="btn btn-primary btn-lg btn-block"
         data-toggle="collapse" data-target="#keyword-options">Pengaturan</button>
      </div>
    </div>

    <div id="keyword-options" class="collapse">
      <div class="form-group">
        <label for="algoritma" class="control-label col-sm-3 light-text">Algoritma</label>
        <div class="col-sm-9">
          <label class="radio-inline light-text">
            <input type="radio" name="algoritma" value="kmp" checked> Knuth-Morris-Pratt
          </label>
          <label class="radio-inline light-text">
            <input type="radio" name="algoritma" value="bm"> Boyer-Moore
          </label>
        </div>
      </div>
      <div class="form-group">
        <label for="dinas1" class="control-label col-sm-3 light-text">Dinas Bina Marga dan Pengairan</label>
        <div class="col-sm-9">
          <input type="text" name="dinas1" class="form-control"
          value="pipa, air, saluran, banjir, lampu, padam, pdam">
        </div>
      </div>
      <div class="form-group">
        <label for="dinas2" class="control-label col-sm-3 light-text">Dinas Kesehatan</label>
        <div class="col-sm-9">
          <input type="text" name="dinas2" class="form-control"
          value="sakit, nyamuk, luka, bpjs, penyakit, sehat">
        </div>
      </div>
      <div class="form-group">
        <label for="dinas3" class="control-label col-sm-3 light-text">Dinas Pendidikan</label>
        <div class="col-sm-9">
          <input type="text" name="dinas3" class="form-control"
          value="sekolah, kampus, ijasah, ijazah, pelajar, siswa">
        </div>
      </div>
      <div class="form-group">
        <label for="dinas4" class="control-label col-sm-3 light-text">Dinas Perhubungan</label>
        <div class="col-sm-9">
          <input type="text" name="dinas4" class="form-control"
          value="angkot, macet, perempatan, ngetem, damri, kereta, bandara, pesawat">
        </div>
      </div>
      <div class="form-group">
        <label for="dinas5" class="control-label col-sm-3 light-text">Dinas Kebersihan</label>
        <div class="col-sm-9">
          <input type="text" name="dinas5" class="form-control"
          value="sampah, pohon, kotor, bersih, bau, busuk">
        </div>
      </div>
    </div>

  </form>

  <br>

  <div id="result" class="no-result">
    <div id="selector-buttons">
      <div class="row">
        <div class="col-sm-4"><a data-toggle="pill" href="#tweet-dinas1" class="btn btn-lg btn-block btn-primary active">
          Dinas Bina Marga dan Pengairan <span class="badge" id="count-dinas1"></span>
        </a></div>
        <br class="visible-xs-inline">
        <div class="col-sm-4"><a data-toggle="pill" href="#tweet-dinas2" class="btn btn-lg btn-block btn-primary">
          Dinas Kesehatan <span class="badge" id="count-dinas2"></span>
        </a></div>
        <br class="visible-xs-inline">
        <div class="col-sm-4"><a data-toggle="pill" href="#tweet-dinas3" class="btn btn-lg btn-block btn-primary">
          Dinas Pendidikan <span class="badge" id="count-dinas3"></span>
        </a></div>
      </div>
      <br>
      <div class="row">
        <div class="col-sm-4"><a data-toggle="pill" href="#tweet-dinas4" class="btn btn-lg btn-block btn-primary">
          Dinas Perhubungan <span class="badge" id="count-dinas4"></span>
        </a></div>
        <br class="visible-xs-inline">
        <div class="col-sm-4"><a data-toggle="pill" href="#tweet-dinas5" class="btn btn-lg btn-block btn-primary">
          Dinas Kebersihan <span class="badge" id="count-dinas5"></span>
        </a></div>
        <br class="visible-xs-inline">
        <div class="col-sm-4"><a data-toggle="pill" href="#tweet-tidakjelas" class="btn btn-lg btn-block btn-info">
          Tidak Jelas <span class="badge" id="count-tidakjelas"></span>
        </a></div>
      </div>
    </div>
    <br>

    <div id="tweet-list" class="tab-content panel-body">
      <div id="tweet-dinas1" class="list-group tab-pane active"></div>
      <div id="tweet-dinas2" class="list-group tab-pane"></div>
      <div id="tweet-dinas3" class="list-group tab-pane"></div>
      <div id="tweet-dinas4" class="list-group tab-pane"></div>
      <div id="tweet-dinas5" class="list-group tab-pane"></div>
      <div id="tweet-tidakjelas" class="list-group tab-pane"></div>
    </div>

  </div>

</div>

<script>window.twttr = (function(d, s, id) {
  var js, fjs = d.getElementsByTagName(s)[0],
    t = window.twttr || {};
  if (d.getElementById(id)) return t;
  js = d.createElement(s);
  js.id = id;
  js.src = "https://platform.twitter.com/widgets.js";
  fjs.parentNode.insertBefore(js, fjs);

  t._e = [];
  t.ready = function(f) {
    t._e.push(f);
  };

  return t;
}(document, "script", "twitter-wjs"));</script>
<script src="https://agilajah.github.io/tweetdis/Content/jquery.min.js" charset="utf-8"></script>
<script src="https://agilajah.github.io/tweetdis/Content/bootstrap.min.js" charset="utf-8"></script>
<script src="https://agilajah.github.io/tweetdis/Content/main.js" charset="utf-8"></script>
