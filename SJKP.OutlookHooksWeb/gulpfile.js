var gulp = require('gulp');
var wiredep = require('wiredep').stream;
var rename = require('gulp-rename');

gulp.task('bower', function () {
    gulp.src('./index.pre.html')
      .pipe(wiredep({       
      }))
      .pipe(rename('index.html'))
      .pipe(gulp.dest('./'));
});