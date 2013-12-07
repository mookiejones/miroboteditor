package com.mookie.prototype.friends;

import android.os.Bundle;
import android.support.v4.app.Fragment;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;

import com.mookie.android.ui.fragment.MasterDetailFragmentHelper;
import com.mookie.android.ui.fragment.MasterDetailFragments;
import com.mookie.prototype.R;

public class FriendMainFragment extends Fragment {

	@Override
	public void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
        setRetainInstance(false);
	}
	
	@Override
	public View onCreateView(LayoutInflater inflater, ViewGroup container,
			Bundle savedInstanceState) {
        View view = inflater.inflate(R.layout.friends, container, false);

        MasterDetailFragments currentFragments = MasterDetailFragmentHelper
                .getCurrentFragments(R.id.friendmain_fragment,
                        R.id.frienddetail_fragment, FriendDetailFragment.class,
                        getChildFragmentManager());
        if ( currentFragments.master == null ) {
            currentFragments.master = FriendListFragment.newInstance();
        }

        MasterDetailFragmentHelper.initFragments(currentFragments, R.id.friendmain_fragment,
                R.id.frienddetail_fragment, getResources().getConfiguration(), getChildFragmentManager());

		return view ;
	}
}
