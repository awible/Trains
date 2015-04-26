using System;
using System.Collections;
using System.IO;


// Restrictions/Comments of importance
// 1. No recovery/exception processing exists in this application. If it was important, I'd add it.
// 2. Recursion is used in some processes. If I was worried about performance, I'd refine the application
//    to process using a stack instead of making recursive calls (to avoid the overhead of allocating local
//    variables on each recursive call).
// 3. Single character station names are defined in the problem definition. Modifying the application to 
//    have normal names would be straightforward (mostly in the area of input file processing, and parameters to
//    GetDistance - which takes a simple 3-character string).
// 4. The application could be modified fairly easily to take an input file of problems (e.g. distance from
//    A to B to C). Since the problem was defined specifically, I felt it was not important to go through hoops
//    to create a construct for this. (Creating the file processing for the input tracks was easy because I had
//    already written code for another application that was fairly close). 
// 5. I didn't go through and delineate what should be private and public as closely as I should have.
namespace Trains
{
	/// <summary>
	/// Summary description for TrainSystem.
	/// </summary>
	public class TrainSystem
	{
		// collection of stations in the system
		private Stations mStations;

		// collection of tracks in the system
		// this is public only for debugging purposes (i.e. the UI interrogates this to list the tracks)
		public Tracks mTracks;

		// a static variable used in recursive routines that count the number of routes
		private static int numRoutes;

		// a static variable used in recursive routines that track the shortest distance
		private static int shortestDistance;

		// constructor - takes a filename with lines of the following format
		// xyn, where x and y are single char names of train stations, and n is the distance between them
		// Note that there is no error checking in reading the file 
		public TrainSystem(string strFileName)
		{
			// Instantiate an empty stations collection
			mStations = new Stations();

			// Instantiate an empty tracks collection
			mTracks = new Tracks();

			// Open the file for reading as a stream
			StreamReader fsIn = File.OpenText(strFileName);

			// local variable to hold the string representing each track
			string strTrack;

			// local variables to hold the source and target station objects
			Station sourceStation, targetStation;

			// Loop through each string in the input file, setting str to each string
			while ((strTrack=fsIn.ReadLine()) != null) 
			{
				// If the source station doesn't already exist in the station collection, add it
				// Also - set a local variable to represent the source station
				if (!mStations.Exists(strTrack[0]))
				{
					Station newStation = new Station(strTrack[0]);
					mStations.Add(newStation);
					sourceStation = newStation;
				}
				// else, the source station exists - simply retrieve it from the collection
				else
					sourceStation = mStations.GetStation(strTrack[0]);

				// If the target station doesn't already exist in the station collection, add it
				// Also - set a local variable to represent the target station
				if (!mStations.Exists(strTrack[1]))
				{
					Station newStation = new Station(strTrack[1]);
					mStations.Add(newStation);
					targetStation = newStation;
				}
				// else, the target station exists - simply retrieve it from the collection
				else
					targetStation = mStations.GetStation(strTrack[1]);

				// Create a new track object to represent the track
				// Get the distance by converting the 1-character representation of the distance to a value
				int distance = Convert.ToByte(strTrack[2]) - Convert.ToByte('0');
				Track newTrack = new Track(sourceStation, targetStation, distance);

				// Add the track to the track collection
				mTracks.Add(newTrack);

				// Add the track to the outbound track collection for the source station
				sourceStation.AddTrack(newTrack);
			}

			// Close the file
			fsIn.Close();

		}

		// Take a string of station names and compute the distance along the route
		// For example, passing stations="ABDE" will compute the distance along the route that
		// goes from A to B to D and then to E
		// If no route exists, return 0
		public int GetDistance(string stations)
		{
			// Set the start station to the first character in the string
			char startStation = stations[0];

			// Initialize the distance
			int distance = 0;

			// Loop through the rest of the stations in the list, starting with the second station
			foreach (char destination in stations.Substring(1))
			{
				// Get the track from the start station to the next station
				Track track = mTracks.getTrack(startStation, destination);

				// If it doesn't exist, return 0 indicating no such route
				if (track == null)
					return 0;

				// Otherwise, add the distance for this track
				distance += track.Distance;

				// Reset the start station to the current destination station, so that the next iteration will
				// add the track starting from the destination
				startStation = destination;
			}

			// return the calculated distance
			return distance;
		}

		// Get the number of routes between two stations that has exactly the number of stops specified
		public int GetNumRoutesWithNStops(char startStation, char endStation, int numStops)
		{
			// initialize static variable representing the number of routes
			numRoutes = 0;

			// call the recursive routine to calculate the number of routes
			return this.GetNumRoutesWithNStopsRecurse(startStation, endStation, numStops);
		}

		// Recursive routine to find the number of routes that have the specific number of stops specified
		// startStation varies with each recursion, to represent the next stop along the route
		// endStation is always the last station in which to end
		// numStops decrements with each recursion
		private int GetNumRoutesWithNStopsRecurse(char startStation, char endStation, int numStops)
		{
			// If only one stop remains, check to see if there is a track from the current startStation to
			// the endStation. If so, we have found a route with the number of stops required.
			if (numStops == 1)
			{
				// If there is a last track to the endStation, increment the number of routes and return
				if (mTracks.getTrack(startStation, endStation) != null)
				{
					numRoutes += 1;
					return numRoutes;
				}
				// else there is no last track to this endStation; simply return the same numRoutes value passed
				else
					return numRoutes;
			}
			// otherwise there is more than one stop possible in the remaining routes
			else
			{
				// Instantiate a new start station object from the startStation name (one character)
				Station start = this.mStations.GetStation(startStation);

				// Loop through each outbound track from this station and try to find a route with the remaining
				// number of stops available (i.e. n-1 from the current number of stops, since we have to account
				// for this track having been traversed)
				// The new starting station is the destination station from each outbound track.
				foreach (Track track in start.outboundTracks)
					numRoutes = GetNumRoutesWithNStopsRecurse(track.Target.Name, endStation, numStops-1);
			}
			// return the result
			return numRoutes;
		}

		// Get the number of routes between two stations that have less than the specified number of
		// stops.
		public int GetAllRoutesWithMaxStops(char startStation, char endStation, int maxStops)
		{
			// initialize static variable representing the number of routes
			numRoutes = 0;

			// call the recursive routine to calculate the number of routes
			return GetAllRoutesWithMaxStopsRecurse(startStation, endStation, maxStops);
		}

		// Recursive routine to find the number of routes that less than the specific number of stops specified
		// startStation varies with each recursion, to represent the next stop along the route
		// endStation is always the last station in which to end
		// maxStops decrements with each recursion
		private int GetAllRoutesWithMaxStopsRecurse(char startStation, char endStation, int maxStops)
		{
			// if this iteration only has one stop left to use under the max stops limit,
			// check to see if there is a track from the current start to the destination
			// if so, increment the number of routes that are less than the max number of stops
			if (maxStops == 1)
			{
				if (mTracks.getTrack(startStation, endStation) != null)
				{
					numRoutes += 1;
					return numRoutes;
				}
				// else, there is no final track to the destination; simply return the current number of routes
				else
					return numRoutes;
			}
			// else, there is more than one possible stop left to check under the max stops limit
			else
			{
				// First, check to see if there is a route from the current start to the final destination
				// If so, increment the number of routes
				if (mTracks.getTrack(startStation, endStation) != null)
					{
						numRoutes += 1;
					}
				// Next, Instantiate a new start station object from the startStation name (one character)
				Station start = this.mStations.GetStation(startStation);

				// Loop through each outbound track from this station and try to find a route under the remaining
				// number of stops available (i.e. n-1 from the current number of stops, since we have to account
				// for this track having been traversed)
				// The new starting station is the destination station from each outbound track.
				foreach (Track track in start.outboundTracks)
					numRoutes = GetAllRoutesWithMaxStopsRecurse(track.Target.Name, endStation, maxStops-1);
			}

			// return the result
			return numRoutes;
		}

		// Calculate the shortest route between two stations
		public int GetShortestRoute(char startStation, char endStation)
		{
			// Initialize the static variable tracking the current shortest track to some very high number
			shortestDistance = 999;

			// In order to avoid endless loops, we need to remember stations visited.
			// This "visitedStations" variable tracks the stations visited on any particular traversal
			// It is initialized to empty (since no stations have yet been visited).
			Stations visitedStations = new Stations();

			// call the recursive routine to calculate the shortest route
			// 0 represents the current route length
			// visitedStations is initially empty
			return GetShortestRouteRecurse(startStation, endStation, 0, visitedStations);
		}

		// Recursive routine to find the shortest route 
		// startStation varies with each recursion, to represent the next stop along the route
		// endStation is always the last station in which to end
		// Current Distance is incremented on each iteration to track the total distance along the route
		// vistedStations rembembers stations visited in the current iteration (required to prevent infinite loops)
		private int GetShortestRouteRecurse(char startStation, char endStation, int currentDistance, Stations visitedStations)
		{
			// See if there is a track from the current start station to the ultimate destination
			Track track = mTracks.getTrack(startStation,endStation);

			// If there is a track to the destination, check to see if adding this track makes the route distance 
			// shorter than the currently known shortest distance. If it's shorter, remember it.
			// Note: don't return at this point, since there could still be a shorter route to the destination through
			// another route (e.g. A-B has weight 9, but A-C-B has weight 4).
			if (track != null)
			{
				if (currentDistance + track.Distance < shortestDistance)
					shortestDistance = currentDistance + track.Distance;
			}

			// At this point, there was no path to the final destination, or the direct route was not shorter than the
			// currently known shortest distance, or it may have been shorter, but we still need to find other paths from
			// this station to see if there is a shorter final set of tracks
			// Instantiate a new start station object from the startStation name (one character)
			Station start = this.mStations.GetStation(startStation);

			// Loop through each outbound track from this station and try to find a route with shorter distance.
			// The new starting station is the destination station from each outbound track.
			foreach (Track outboundTrack in start.outboundTracks)
			{		
				// if we've already visited the target station, there's no point in considering going there again.
				// Only visit stations that haven't already been visited.
				if (!visitedStations.Exists(outboundTrack.Target))
				{
					// Remember that the target station has been visited (since we're about to visit it).
					visitedStations.Add(outboundTrack.Target);

					// Recurse, adding the current track distance
					shortestDistance = GetShortestRouteRecurse(outboundTrack.Target.Name, endStation, currentDistance+outboundTrack.Distance, visitedStations);

					// Now that the target station has been "popped" from the stack, remove it from the list of 
					// visited stations
					visitedStations.Remove(outboundTrack.Target);
				}
			}

			// Return result
			return shortestDistance;
		}

		// Get the number of routes from point A to point B that have a distance less than the provided value
		public int GetAllRoutesWithMaxDistance(char startStation, char endStation, int maxDistance)
		{
			// initialize static variable representing the number of routes
			numRoutes = 0;

			// call the recursive routine to calculate the number of routes			
			return GetAllRoutesWithMaxDistanceRecurse(startStation, endStation, maxDistance, 0);
		}
		private int GetAllRoutesWithMaxDistanceRecurse(char startStation, char endStation, int maxDistance, int currentDistance)
		{
			// See if there is a track from the current start station to the ultimate destination
			Track track = mTracks.getTrack(startStation, endStation);

			// If there is a track to the destination, check to see if adding this track makes the route distance 
			// shorter than the max distance. If it's shorter, increment the number of routes under the specified max.
			// Note: don't return at this point, since there could still be more routes to the destination through
			// another route.
			if (track != null)
			{
				if (currentDistance + track.Distance < maxDistance)
					numRoutes += 1;
				else
					return numRoutes;
			}

			// Next, Instantiate a new start station object from the startStation name (one character)
			Station start = this.mStations.GetStation(startStation);
			
			// Loop through each outbound track from this station and try to find routes under the remaining
			// distance available (i.e. currDistance-track.Distance from the current distance traversed, since we 
			// have to account for this track having been traversed)
			// The new starting station is the destination station from each outbound track.
			foreach (Track outboundTrack in start.outboundTracks)
				if (currentDistance + outboundTrack.Distance < maxDistance)
					numRoutes = GetAllRoutesWithMaxDistanceRecurse(outboundTrack.Target.Name, endStation, maxDistance, currentDistance+outboundTrack.Distance);

			// Return result
			return numRoutes;
		}

	}

	// Stations is a collection to represent a list of train stations
	public class Stations : ArrayList
	{
		// Exists: return true if the Station object exists in the collection
		public bool Exists(Station queryStation)
		{
			if (this.Exists(queryStation.Name))
				return true;
			else
				return false;
		}
	
		// Exists: return true if the Station object with the specified name exists in the collection
		public bool Exists(char queryStationName)
		{
			foreach (Station station in this)
			{
				if (queryStationName == station.Name)
					return true;
			}
			return false;
		}

		// GetStation: returns a Station object with the specified name (or throws an exception if the station
		// doesn't exist
		public Station GetStation (char stationName)
		{
			foreach (Station station in this)
				if (station.Name == stationName)
					return station;
			throw (new Exception("Attempt to retrieve a nonexistent Station"));
		}
	}

	// Station class represents a train station
	public class Station
	{
		// Name of station
		private char mName;

		// Tracks that lead from this station
		public Tracks outboundTracks;

		// Constructor - takes a one-char name of a train station
		public Station(char name)
		{
			mName = name;
			outboundTracks = new Tracks();
		}

		// Add an outbound track from this station
		public void AddTrack(Track track)
		{
			outboundTracks.Add(track);
		}

		// Getter method to return the station name
		public char Name
		{
			get
			{
				return mName;
			}
		}
	
	}

	// Tracks represents the collection of tracks in the system
	public class Tracks : ArrayList
	{
		// Given a source and target station name, returns the track object representing the track from
		// the source to the target
		// returns null if no track exists from the source to the target
		public Track getTrack (char querySourceStationName, char queryTargetStationName)
		{
			foreach (Track track in this)
			{
				if (querySourceStationName == track.Source.Name &&
					queryTargetStationName == track.Target.Name)
					return track;
			}
			return null;
		}

		// Return the distance between two stations - or 0 if the track doesn't exist
		private int GetDistance(char querySourceStationName, char queryTargetStationName)
		{
			Track queryTrack = this.getTrack(querySourceStationName, queryTargetStationName);
			if (queryTrack == null)
				return 0;
			else
				return queryTrack.Distance; 
		}
	}

	// Track represents a track in the system - source station, target station, and distance
	public class Track
	{
		private Station mSource, mTarget;
		private int mDistance;

		// Constructor
		public Track(Station source, Station target, int distance)
		{
			mSource = source;
			mTarget = target;
			mDistance = distance;
		}

		// return the source station
		public Station Source
		{
			get
			{
				return mSource;
			}
		}
		
		// return the target station
		public Station Target
		{
			get
			{
				return mTarget;
			}
		}

		// return the track distance
		public int Distance
		{
			get
			{
				return mDistance;
			}
		}
	}
}
