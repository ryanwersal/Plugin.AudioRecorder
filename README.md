# Audio Recorder plugin for Xamarin and Windows ![NuGet](https://img.shields.io/nuget/v/Plugin.AudioRecorder.svg?label=NuGet)

Records audio on a device's microphone input.

# Setup

- Available on NuGet: https://www.nuget.org/packages/Plugin.AudioRecorder
- Install into your PCL project and any platform-specific libraries required for your app.

## Platform Support

|Platform|Supported|Version|
| ------------------- | :-----------: | :------------------: |
|Xamarin.iOS|Yes|iOS 7+|
|Xamarin.iOS Unified|Yes|iOS 7+|
|Xamarin.Android|Yes|API 16+|
|Windows Phone Silverlight|No||
|Windows Phone RT|No||
|Windows Store RT|No||
|Windows 10 UWP|Yes|10+|
|Xamarin.Mac|No||

**_Supports both classic Xamarin.iOS / Xamarin.Android and Xamarin.Forms_**

### Permissions

The following permissions are required on each platform:

#### Android

```XML
<uses-permission android:name="android.permission.MODIFY_AUDIO_SETTINGS" />
<uses-permission android:name="android.permission.READ_EXTERNAL_STORAGE" />
<uses-permission android:name="android.permission.WRITE_EXTERNAL_STORAGE" />
<uses-permission android:name="android.permission.RECORD_AUDIO" />
```

#### iOS

```XML
<key>NSMicrophoneUsageDescription</key>
<string>The [app name] wants to use your microphone to record audio.</string>
```

#### UWP

You must check the `Internet` and `Microphone` capabilities in your app's Package.appxmanifest file.


# Usage

In a controller/activity/page, initialize a new `AudioRecorderService`.

Example:

```C#
recorder = new AudioRecorderService
{
	StopRecordingOnSilence = true, //will stop recording after 2 seconds (default)
	StopRecordingAfterTimeout = true,  //stop recording after a max timeout (defined below)
	TotalAudioTimeout = TimeSpan.FromSeconds (15) //audio will stop recording after 15 seconds
};
```

More settings and properties are [defined below](#properties-and-settings)

## Recording

To begin recording, use the `StartRecording ()` and `StopRecording ()` methods as shown:

```C#
async void RecordButton_Click (object sender, EventArgs e)
{
	await RecordAudio ();
}

async Task RecordAudio ()
{
	try
	{
		if (!recorder.IsRecording)
		{
			await recorder.StartRecording ();
		}
		else
		{
			await recorder.StopRecording ();
		}
	}
	catch (Exception ex)
	{
	...
	}
}
```

In lieu of calling `StopRecording ()`, you can also make use of the `StopRecordingAfterTimeout` and/or `StopRecordingOnSilence` settings, which are [explained below](#properties-and-settings).


## Using the Audio Data

Once recording has begun, there are two different ways to determine when recording has finished:

### Task-based API

To use the Task-based API, you can grab the returned `Task` from the call to `StartRecording ()`.  This allows you to await the result of the `Task`, which will complete when recording is complete and return the path to the recorded audio file.

Example:

```C#
var recordTask = await recorder.StartRecording ();

... maybe do some other things like toggle your 'mic' button off while recording

//await the returned Task... this will complete once recording has been stopped
var audioFile = await recordTask;

if (audioFile != null) //non-null audioFile indicates audio was successfully recorded
{
	//do something with the file
}
```


### Event-based API

The `AudioInputReceived` is raised when recording is complete, and the full filepath of the recorded audio file is passed along.

Example:

```C#
recorder.AudioInputReceived += Recorder_AudioInputReceived;

...

await recorder.StartRecording ();

...

private async void Recorder_AudioInputReceived(object sender, string audioFile)
{
	//do something with the file
}
```

**NOTE:** This event is raised on a background thread to allow for further file processing as needed.  If the `audioFile` is null or empty, no audio was recorded.

--

There are also multiple ways to use the recorded (or recording) audio data:


### Accessing the Recorded File

There are multiple ways to access the recorded audio file path:

- The [Task-based API](#task-based-api) will return the file path when the task completes.  The `Task` can be awaited or use standard `Task` continuation APIs.
- The [Event-based API](#event-based-api) will return the full path to the recorded audio file in the `audioFile` parameter of the `AudioInputReceived` event handler.
- The `GetAudioFilePath ()` method on the `AudioRecorderService ` class will return the recorded audio file path.

These will all return `null` in the case that no audio has been recorded yet or no audio was recorded/detected in the last recording session.

Once you have the path to the recorded audio file, you can use standard `FileStream` operations and/or a cross platform file system abstraction like [PCLStorage](https://github.com/dsplaisted/PCLStorage) to get a stream to the file data.

Complete samples demonstrating audio recording and playback of the recorded file are available in the /Samples folder.


### Concurrent Streaming

It's also possible to get a stream to the recording audio data as it's being recorded, once `StartRecording ()` has been called.

To access this readonly stream of audio data, you may call the `GetAudioFileStream ()` method.  This is useful in the case you want to immediately begin streaming the audio data to a server or other consumer.

**NOTE:** Since the WAV header is written after recording, once the audio length is known, the provided `Stream` data will contain the PCM audio data only and will **not** contain a WAV header.  If your use case requires a WAV header, you can call `AudioFunctions.WriteWaveHeader (Stream stream, int channelCount, int sampleRate, int bitsPerSample)`, which will write a WAV header to the stream with an unknown length.

Since `GetAudioFileStream ()` will return a `Stream` that is also being populated concurrently, it can be useful to know when the recording is complete - the `Stream` will continue to grow!  This can be accomplished with either the [Event-based API](#event-based-api) or the [Task-based API](#task-based-api) (which is often more useful).

An example of the Task-based API and concurrent writing and reading of the audio data is shown in the sample accompanying the [Xamarin.Cognitive.Speech](https://github.com/NateRickard/Xamarin.Cognitive.BingSpeech) library.  This speech client will stream audio data to the server until the `AudioRecordTask` completes, signaling that the recording is finished.


## Properties and Settings


- IsRecording

	```C#
	bool IsRecording
	```

	Returns a value indicating if the AudioRecorderService is currently recording audio.

- StopRecordingAfterTimeout / TotalAudioTimeout

	```C#
	bool StopRecordingAfterTimeout
	```
	
	Gets/sets a value indicating if the AudioRecorderService should stop recording after a certain amount of time.  Default is `true`.

	```C#
	TimeSpan TotalAudioTimeout
	```

	_If_ `StopRecordingAfterTimeout` is set to `true`, this `TimeSpan` indicates the total amount of time to record audio for before recording is stopped. Defaults to 30 seconds.

- StopRecordingOnSilence / AudioSilenceTimeout

	```C#
	bool StopRecordingOnSilence
	```
	
	Gets/sets a value indicating if the AudioRecorderService should stop recording after silence (low audio signal) is detected.  Default is `true`.
	
	```C#
	TimeSpan AudioSilenceTimeout
	```
	_If_ `StopRecordingOnSilence ` is set to `true`, this `TimeSpan` indicates the amount of 'silent' time is required before recording is stopped. Defaults to 2 seconds.

- SilenceThreshold

	```C#
	float SilenceThreshold
	```
	
	Gets/sets a value indicating the signal threshold that determines silence.  If the recorder is being over or under aggressive when detecting silence, you can alter this value to achieve different results.  Defaults to .2. Value should be between 0 and 1.

- FilePath

	```C#
	string FilePath
	```

	Gets/sets the desired file path. If null it will be set automatically to a temporary file.


# Limitations

- Currently this is only recording in WAV audio format (due to original use case this was developed for).
- Signal detection (`StopRecordingOnSilence`) is not currently working well/at all on UWP.


# Contributing

Contributions are welcome.  Feel free to file issues and pull requests on the repo and they'll be reviewed as time permits.


# Building NuGet Package

Invoke the `build.cmd` script in a Visual Studio Developer Command Prompt. A nupkg file will be built and copied into the root of the project directory (alongside the solution).


# About

- Created by [Nate Rickard](https://github.com/naterickard)
- AudioRecorderService concept and some original code from [SmartCoffee](https://github.com/pierceboggan/smartcoffee) by [Pierce Boggan](https://github.com/pierceboggan)
- Audio stream and wave recorder classes adapted from [SimplyMobile](https://github.com/sami1971/SimplyMobile) by [Sami M. Kallio](https://github.com/sami1971)


## License

Licensed under the MIT License (MIT). See [LICENSE](LICENSE) for details.