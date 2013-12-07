package com.mookie.prototype;

import com.mookie.android.MCXModule;
import com.mookie.prototype.donations.DonateFragment;
import com.mookie.prototype.friends.FriendContentProvider;
import com.mookie.prototype.home.MainFragment;
import com.mookie.prototype.home.YourAppMainActivity;
import com.mookie.prototype.settings.SettingsFragment;
import com.mookie.prototype.tutorial.TutorialListFragment;
import com.mookie.prototype.tutorial.contentprovider.TutorialContentProvider;
import com.mookie.prototype.tutorial.sync.TutorialSyncAdapter;

import dagger.Module;

@Module(
	    injects = {
            YourApplication.class,

            YourAppMainActivity.class,

    		FriendContentProvider.class,
            TutorialContentProvider.class,

            MainFragment.class,
            DonateFragment.class,
            SettingsFragment.class,
            TutorialListFragment.class,

            TutorialSyncAdapter.class
	    },
	    includes = {
	    	MCXModule.class
	    },
	    overrides=true
	)
public class YourAppModule {

}
